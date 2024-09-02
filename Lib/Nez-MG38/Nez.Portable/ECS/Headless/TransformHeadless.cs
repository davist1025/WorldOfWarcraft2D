using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nez.ECS.Headless
{
	public class TransformHeadless
	{
		[Flags]
		enum DirtyType
		{
			Clean = 0,
			PositionDirty = 1,
			ScaleDirty = 2,
			RotationDirty = 4
		}

		public enum Component
		{
			Position,
			Scale,
			Rotation
		}


		#region properties and fields

		/// <summary>
		/// the Entity associated with this TransformHeadless
		/// </summary>
		public readonly EntityHeadless Entity;

		/// <summary>
		/// the parent TransformHeadless of this TransformHeadless
		/// </summary>
		/// <value>The parent.</value>
		public TransformHeadless Parent
		{
			get => _parent;
			set => SetParent(value);
		}


		/// <summary>
		/// total children of this TransformHeadless
		/// </summary>
		/// <value>The child count.</value>
		public int ChildCount => _children.Count;


		/// <summary>
		/// position of the TransformHeadless in world space
		/// </summary>
		/// <value>The position.</value>
		public Vector2 Position
		{
			get
			{
				UpdateTransform();
				if (_positionDirty)
				{
					if (Parent == null)
					{
						_position = _localPosition;
					}
					else
					{
						Parent.UpdateTransform();
						Vector2Ext.Transform(ref _localPosition, ref Parent._worldTransform, out _position);
					}

					_positionDirty = false;
				}

				return _position;
			}
			set => SetPosition(value);
		}


		/// <summary>
		/// position of the TransformHeadless relative to the parent TransformHeadless. If the TransformHeadless has no parent, it is the same as TransformHeadless.position
		/// </summary>
		/// <value>The local position.</value>
		public Vector2 LocalPosition
		{
			get
			{
				UpdateTransform();
				return _localPosition;
			}
			set => SetLocalPosition(value);
		}


		/// <summary>
		/// rotation of the TransformHeadless in world space in radians
		/// </summary>
		/// <value>The rotation.</value>
		public float Rotation
		{
			get
			{
				UpdateTransform();
				return _rotation;
			}
			set => SetRotation(value);
		}


		/// <summary>
		/// rotation of the TransformHeadless in world space in degrees
		/// </summary>
		/// <value>The rotation degrees.</value>
		public float RotationDegrees
		{
			get => MathHelper.ToDegrees(_rotation);
			set => SetRotation(MathHelper.ToRadians(value));
		}


		/// <summary>
		/// the rotation of the TransformHeadless relative to the parent TransformHeadless's rotation. If the TransformHeadless has no parent, it is the same as TransformHeadless.rotation
		/// </summary>
		/// <value>The local rotation.</value>
		public float LocalRotation
		{
			get
			{
				UpdateTransform();
				return _localRotation;
			}
			set => SetLocalRotation(value);
		}


		/// <summary>
		/// rotation of the TransformHeadless relative to the parent TransformHeadless's rotation in degrees
		/// </summary>
		/// <value>The rotation degrees.</value>
		public float LocalRotationDegrees
		{
			get => MathHelper.ToDegrees(_localRotation);
			set => LocalRotation = MathHelper.ToRadians(value);
		}


		/// <summary>
		/// global scale of the TransformHeadless
		/// </summary>
		/// <value>The scale.</value>
		public Vector2 Scale
		{
			get
			{
				UpdateTransform();
				return _scale;
			}
			set => SetScale(value);
		}


		/// <summary>
		/// the scale of the TransformHeadless relative to the parent. If the TransformHeadless has no parent, it is the same as TransformHeadless.scale
		/// </summary>
		/// <value>The local scale.</value>
		public Vector2 LocalScale
		{
			get
			{
				UpdateTransform();
				return _localScale;
			}
			set => SetLocalScale(value);
		}


		public Matrix2D WorldInverseTransform
		{
			get
			{
				UpdateTransform();
				if (_worldInverseDirty)
				{
					Matrix2D.Invert(ref _worldTransform, out _worldInverseTransform);
					_worldInverseDirty = false;
				}

				return _worldInverseTransform;
			}
		}


		public Matrix2D LocalToWorldTransform
		{
			get
			{
				UpdateTransform();
				return _worldTransform;
			}
		}


		public Matrix2D WorldToLocalTransform
		{
			get
			{
				if (_worldToLocalDirty)
				{
					if (Parent == null)
					{
						_worldToLocalTransform = Matrix2D.Identity;
					}
					else
					{
						Parent.UpdateTransform();
						Matrix2D.Invert(ref Parent._worldTransform, out _worldToLocalTransform);
					}

					_worldToLocalDirty = false;
				}

				return _worldToLocalTransform;
			}
		}


		TransformHeadless _parent;
		DirtyType hierarchyDirty;

		bool _localDirty;
		bool _localPositionDirty;
		bool _localScaleDirty;
		bool _localRotationDirty;
		bool _positionDirty;
		bool _worldToLocalDirty;
		bool _worldInverseDirty;

		// value is automatically recomputed from the position, rotation and scale
		Matrix2D _localTransform;

		// value is automatically recomputed from the local and the parent matrices.
		Matrix2D _worldTransform = Matrix2D.Identity;
		Matrix2D _worldToLocalTransform = Matrix2D.Identity;
		Matrix2D _worldInverseTransform = Matrix2D.Identity;

		Matrix2D _rotationMatrix;
		Matrix2D _translationMatrix;
		Matrix2D _scaleMatrix;

		Vector2 _position;
		Vector2 _scale;
		float _rotation;

		Vector2 _localPosition;
		Vector2 _localScale;
		float _localRotation;

		List<TransformHeadless> _children = new List<TransformHeadless>();

		#endregion


		public TransformHeadless(EntityHeadless entity)
		{
			Entity = entity;
			_scale = _localScale = Vector2.One;
		}


		/// <summary>
		/// returns the TransformHeadless child at index
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="index">Index.</param>
		public TransformHeadless GetChild(int index)
		{
			return _children[index];
		}


		#region Fluent setters

		/// <summary>
		/// sets the parent TransformHeadless of this TransformHeadless
		/// </summary>
		/// <returns>The parent.</returns>
		/// <param name="parent">Parent.</param>
		public TransformHeadless SetParent(TransformHeadless parent)
		{
			if (_parent == parent)
				return this;

			if (_parent != null)
				_parent._children.Remove(this);

			if (parent != null)
				parent._children.Add(this);

			_parent = parent;
			SetDirty(DirtyType.PositionDirty);

			return this;
		}


		/// <summary>
		/// sets the position of the TransformHeadless in world space
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="position">Position.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetPosition(Vector2 position)
		{
			if (position == _position)
				return this;

			_position = position;
			if (Parent != null)
				LocalPosition = Vector2.Transform(_position, WorldToLocalTransform);
			else
				LocalPosition = position;

			_positionDirty = false;

			return this;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetPosition(float x, float y)
		{
			return SetPosition(new Vector2(x, y));
		}


		/// <summary>
		/// sets the position of the TransformHeadless relative to the parent TransformHeadless. If the TransformHeadless has no parent, it is the same
		/// as TransformHeadless.position
		/// </summary>
		/// <returns>The local position.</returns>
		/// <param name="localPosition">Local position.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetLocalPosition(Vector2 localPosition)
		{
			if (localPosition == _localPosition)
				return this;

			_localPosition = localPosition;
			_localDirty = _positionDirty = _localPositionDirty = _localRotationDirty = _localScaleDirty = true;
			SetDirty(DirtyType.PositionDirty);

			return this;
		}


		/// <summary>
		/// sets the rotation of the TransformHeadless in world space in radians
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetRotation(float radians)
		{
			_rotation = radians;
			if (Parent != null)
				LocalRotation = Parent.Rotation + radians;
			else
				LocalRotation = radians;

			return this;
		}


		/// <summary>
		/// sets the rotation of the TransformHeadless in world space in degrees
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetRotationDegrees(float degrees)
		{
			return SetRotation(MathHelper.ToRadians(degrees));
		}


		/// <summary>
		/// sets the the rotation of the TransformHeadless relative to the parent TransformHeadless's rotation. If the TransformHeadless has no parent, it is the
		/// same as TransformHeadless.rotation
		/// </summary>
		/// <returns>The local rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetLocalRotation(float radians)
		{
			_localRotation = radians;
			_localDirty = _positionDirty = _localPositionDirty = _localRotationDirty = _localScaleDirty = true;
			SetDirty(DirtyType.RotationDirty);

			return this;
		}


		/// <summary>
		/// sets the the rotation of the TransformHeadless relative to the parent TransformHeadless's rotation. If the TransformHeadless has no parent, it is the
		/// same as TransformHeadless.rotation
		/// </summary>
		/// <returns>The local rotation.</returns>
		/// <param name="radians">Radians.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetLocalRotationDegrees(float degrees)
		{
			return SetLocalRotation(MathHelper.ToRadians(degrees));
		}

		/// <summary>
		/// Rotate so the top of the sprite is facing <see cref="pos"/>
		/// </summary>
		/// <param name="pos">The position to look at</param>
		public void LookAt(Vector2 pos)
		{
			var sign = _position.X > pos.X ? -1 : 1;
			var vectorToAlignTo = Vector2.Normalize(_position - pos);
			Rotation = sign * Mathf.Acos(Vector2.Dot(vectorToAlignTo, Vector2.UnitY));
		}

		/// <summary>
		/// sets the global scale of the TransformHeadless
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetScale(Vector2 scale)
		{
			_scale = scale;
			if (Parent != null)
				LocalScale = scale / Parent._scale;
			else
				LocalScale = scale;

			return this;
		}


		/// <summary>
		/// sets the global scale of the TransformHeadless
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetScale(float scale)
		{
			return SetScale(new Vector2(scale));
		}


		/// <summary>
		/// sets the scale of the TransformHeadless relative to the parent. If the TransformHeadless has no parent, it is the same as TransformHeadless.scale
		/// </summary>
		/// <returns>The local scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetLocalScale(Vector2 scale)
		{
			_localScale = scale;
			_localDirty = _positionDirty = _localScaleDirty = true;
			SetDirty(DirtyType.ScaleDirty);

			return this;
		}


		/// <summary>
		/// sets the scale of the TransformHeadless relative to the parent. If the TransformHeadless has no parent, it is the same as TransformHeadless.scale
		/// </summary>
		/// <returns>The local scale.</returns>
		/// <param name="scale">Scale.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TransformHeadless SetLocalScale(float scale)
		{
			return SetLocalScale(new Vector2(scale));
		}

		#endregion


		/// <summary>
		/// rounds the position of the TransformHeadless
		/// </summary>
		public void RoundPosition()
		{
			Position = Vector2Ext.Round(_position);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void UpdateTransform()
		{
			if (hierarchyDirty != DirtyType.Clean)
			{
				if (Parent != null)
					Parent.UpdateTransform();

				if (_localDirty)
				{
					if (_localPositionDirty)
					{
						Matrix2D.CreateTranslation(_localPosition.X, _localPosition.Y, out _translationMatrix);
						_localPositionDirty = false;
					}

					if (_localRotationDirty)
					{
						Matrix2D.CreateRotation(_localRotation, out _rotationMatrix);
						_localRotationDirty = false;
					}

					if (_localScaleDirty)
					{
						Matrix2D.CreateScale(_localScale.X, _localScale.Y, out _scaleMatrix);
						_localScaleDirty = false;
					}

					Matrix2D.Multiply(ref _scaleMatrix, ref _rotationMatrix, out _localTransform);
					Matrix2D.Multiply(ref _localTransform, ref _translationMatrix, out _localTransform);

					if (Parent == null)
					{
						_worldTransform = _localTransform;
						_rotation = _localRotation;
						_scale = _localScale;
						_worldInverseDirty = true;
					}

					_localDirty = false;
				}

				if (Parent != null)
				{
					Matrix2D.Multiply(ref _localTransform, ref Parent._worldTransform, out _worldTransform);

					_rotation = _localRotation + Parent._rotation;
					_scale = Parent._scale * _localScale;
					_worldInverseDirty = true;
				}

				_worldToLocalDirty = true;
				_positionDirty = true;
				hierarchyDirty = DirtyType.Clean;
			}
		}


		/// <summary>
		/// sets the dirty flag on the enum and passes it down to our children
		/// </summary>
		/// <param name="dirtyFlagType">Dirty flag type.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void SetDirty(DirtyType dirtyFlagType)
		{
			if ((hierarchyDirty & dirtyFlagType) == 0)
			{
				hierarchyDirty |= dirtyFlagType;

				switch (dirtyFlagType)
				{
					case DirtyType.PositionDirty:
						Entity.OnTransformChanged(Component.Position);
						break;
					case DirtyType.RotationDirty:
						Entity.OnTransformChanged(Component.Rotation);
						break;
					case DirtyType.ScaleDirty:
						Entity.OnTransformChanged(Component.Scale);
						break;
				}

				// dirty our children as well so they know of the changes
				for (var i = 0; i < _children.Count; i++)
					_children[i].SetDirty(dirtyFlagType);
			}
		}


		public void CopyFrom(TransformHeadless TransformHeadless)
		{
			_position = TransformHeadless.Position;
			_localPosition = TransformHeadless._localPosition;
			_rotation = TransformHeadless._rotation;
			_localRotation = TransformHeadless._localRotation;
			_scale = TransformHeadless._scale;
			_localScale = TransformHeadless._localScale;

			SetDirty(DirtyType.PositionDirty);
			SetDirty(DirtyType.RotationDirty);
			SetDirty(DirtyType.ScaleDirty);
		}


		public override string ToString()
		{
			return string.Format(
				"[TransformHeadless: parent: {0}, position: {1}, rotation: {2}, scale: {3}, localPosition: {4}, localRotation: {5}, localScale: {6}]",
				Parent != null, Position, Rotation, Scale, LocalPosition, LocalRotation, LocalScale);
		}
	}
}

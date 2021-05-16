using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ShareClientForm
{
    public class ContorlStyle
    {
        private readonly Action<Control> _StyleSetter;
        public Type TargetType { get; }

        private ContorlStyle _BaseStyle;
        public ContorlStyle BaseStyle
        {
            get => _BaseStyle;
            set
            {
                if (value == null || !TargetType.Equals(value.TargetType))
                {
                    throw new InvalidOperationException(value?.TargetType.FullName ?? "value is null");
                }
                _BaseStyle = value;
            }
        }

        public ContorlStyle(Type targetType, Action<Control> styleSetter)
        {
            TargetType = targetType;
            _StyleSetter = styleSetter;
        }

        public void AddStyle(Control target)
        {
            if (target == null || !TargetType.Equals(target.GetType()))
            {
                throw new InvalidOperationException(target?.GetType().FullName ?? "target is null");
            }

            BaseStyle?.AddStyle(target);
            _StyleSetter.Invoke(target);
        }
    }
}

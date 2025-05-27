using System;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public abstract class WheelAdapter
    {
        public WheelPicker? Picker = null;

        public abstract string GetValue(int position);

        public abstract int GetPosition(string value);

        public abstract string GetTextWithMaximumLength();

        public int GetSize()
        {
            return -1;
        }

        public int? GetMinValidIndex()
        {
            return null;
        }

        public int? GetMaxValidIndex()
        {
            return null;
        }

        void NotifyDataSetChanged()
        {
            if (Picker != null)
            {
                Picker.SetAdapter(this);
                Picker.RequestLayout();
            }
        }
    }
}

using System;
using AsyncAwaitBestPractices;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomHuaweiMap : CustomMap
    {
        public double VisibleRegion_Center_Longitude { set; get; }
        public double VisibleRegion_Center_Latitude { set; get; }

        readonly WeakEventManager<CustomPin> addPinEventManager = new WeakEventManager<CustomPin>();
        readonly WeakEventManager<EventArgs> clearPinEventManager = new WeakEventManager<EventArgs>();
        readonly WeakEventManager<MapSpan> moveToRegionEventManager = new WeakEventManager<MapSpan>();
        readonly WeakEventManager<bool> myLocationEventManager = new WeakEventManager<bool>();
        readonly AsyncAwaitBestPractices.WeakEventManager mapReadyEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        public event EventHandler<CustomPin> AddPinEvent
        {
            add => addPinEventManager.AddEventHandler(value);
            remove => addPinEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<EventArgs> ClearPinsEvent
        {
            add => clearPinEventManager.AddEventHandler(value);
            remove => clearPinEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<MapSpan> MoveToRegionEvent
        {
            add => moveToRegionEventManager.AddEventHandler(value);
            remove => moveToRegionEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<bool> MyLocationEnabledEvent
        {
            add => myLocationEventManager.AddEventHandler(value);
            remove => myLocationEventManager.RemoveEventHandler(value);
        }

        public event EventHandler OnMapReadyEvent
        {
            add => mapReadyEventManager.AddEventHandler(value);
            remove => mapReadyEventManager.RemoveEventHandler(value);
        }

        public List<Pin> HMSPins { get; } = new List<Pin>();
        public Pin? HMSSelectedPin { get; set; }

        public override void AddPin(CustomPin pin)
        {
            base.AddPin(pin);

            HMSPins.Add(pin);
            addPinEventManager?.RaiseEvent(this, pin, nameof(AddPinEvent));
        }

        public override void ClearPins()
        {
            base.ClearPins();

            HMSPins.Clear();
            clearPinEventManager?.RaiseEvent(this, new EventArgs(), nameof(ClearPinsEvent));
        }

        public void MoveToRegion(MapSpan span, bool anim = false)
        {
            moveToRegionEventManager?.RaiseEvent(this, span, nameof(MoveToRegionEvent));
        }

        public void HMSMyLocationEnabled(bool isEnable)
        {
            myLocationEventManager?.RaiseEvent(this, isEnable, nameof(MyLocationEnabledEvent));
        }

        public void OnMapReady()
        {
            mapReadyEventManager?.RaiseEvent(this, EventArgs.Empty, nameof(OnMapReadyEvent));
        }
    }
}

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using MAUIEssentials.AppCode.Helpers;
using Application = Android.App.Application;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public class WheelPicker : View
    {
        float TopBottomFadingEdgeStrength = 0.9f;
        int SnapScrollDuration = 300;
        int SelectorMaxFlingVelocityAdjustment = 4;
        int DefaultItemCount = 3;
        int DefaultTextSize = 16;

        float mLastY = 0f;
        float mSelectedTextScale = 0.3f;
        int mCurrentFirstItemOffset = 0;
        int mInitialFirstItemOffset = int.MinValue;

        int? mMinValidIndex, mMaxValidIndex;
        int mSelectorItemCount, mSelectorVisibleItemCount, mMinIndex, mMaxIndex, mWheelMiddleItemIndex,
            mWheelVisibleItemMiddleIndex, mCurrentSelectedItemIndex, mTextSize, mTouchSlop, mMaxVelocity, mMinVelocity,
            mItemHeight, mTextHeight, mTextGapHeight, mPreviousScrollerY;

        WheelItemAlign mTextAlign = WheelItemAlign.Center;
        bool mWrapSelectorWheelPreferred, mIsDragging, mFadingEdgeEnabled = true;

        List<int> mSelectorItemIndices;
        List<bool> mSelectorItemValidStatus;
        Color mTextColor, mSelectedTextColor, mSelectedBackgroundColor;

        IOnValueChangeListener mOnValueChangeListener;
        IOnScrollListener mOnScrollListener;

        Paint mTextPaint = new Paint(PaintFlags.AntiAlias);
        Rect mRect = new Rect();
        WheelAdapter mAdapter;

        OverScroller mOverScroller;
        VelocityTracker mVelocityTracker;

        Typeface mTypeface, mSelectedTypeface;
        WheelScrollState mScrollState = WheelScrollState.Idle;

        Context _context;

        public WheelPicker(Context context) : base(context)
        {
            _context = context;
            Init();
        }

        public WheelPicker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _context = context;
            Init();
        }

        protected WheelPicker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            _context = Platform.AppContext ?? Application.Context;
            Init();
        }

        private void Init()
        {
            mSelectorItemCount = DefaultItemCount + 2;
            mWheelMiddleItemIndex = (mSelectorItemCount - 1) / 2;

            mSelectorVisibleItemCount = mSelectorItemCount - 2;
            mWheelVisibleItemMiddleIndex = (mSelectorVisibleItemCount - 1) / 2;

            mSelectorItemIndices = new List<int>(mSelectorItemCount);
            mSelectorItemValidStatus = new List<bool>(mSelectorItemCount);

            //mMinIndex = int.MinValue;
            //mMaxIndex = int.MaxValue;

            mOverScroller = new OverScroller(_context, new DecelerateInterpolator(2.5f));
            var configuration = ViewConfiguration.Get(_context);
            mTouchSlop = configuration.ScaledTouchSlop;
            mMaxVelocity = configuration.ScaledMaximumFlingVelocity / SelectorMaxFlingVelocityAdjustment;
            mMinVelocity = configuration.ScaledMinimumFlingVelocity;

            mSelectedBackgroundColor = Color.White;
            mSelectedTextColor = Color.Black;
            mTextColor = Color.DarkGray;
            mTextSize = DefaultTextSize;

            InitializeSelectorWheelIndices();
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (changed)
            {
                // need to do all this when we know our size
                InitializeSelectorWheel();
                InitializeFadingEdges();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var lp = LayoutParameters;

            if (lp == null)
            {
                lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }

            var width = CalculateSize(SuggestedMinimumWidth, lp.Width, widthMeasureSpec);
            var height = CalculateSize(SuggestedMinimumHeight, lp.Height, heightMeasureSpec);

            width += PaddingLeft + PaddingRight;
            height += PaddingTop + PaddingBottom;

            SetMeasuredDimension(width, height);
        }

        protected override int SuggestedMinimumWidth
        {
            get
            {
                var suggested = base.SuggestedMinimumHeight;
                if (mSelectorVisibleItemCount > 0)
                {
                    suggested = Math.Max(suggested, ComputeMaximumWidth());
                }
                return suggested;
            }
        }

        protected override int SuggestedMinimumHeight
        {
            get
            {
                var suggested = base.SuggestedMinimumWidth;

                if (mSelectorVisibleItemCount > 0)
                {
                    var fontMetricsInt = mTextPaint.GetFontMetricsInt();
                    var height = fontMetricsInt.Descent - fontMetricsInt.Ascent;
                    suggested = Math.Max(suggested, height * mSelectorVisibleItemCount);
                }
                return suggested;
            }
        }

        private int ComputeMaximumWidth()
        {
            mTextPaint.TextSize = mTextSize * 1.3f;

            if (mAdapter != null)
            {
                if (!string.IsNullOrEmpty(mAdapter.GetTextWithMaximumLength()))
                {
                    var suggestedWith = (int)mTextPaint.MeasureText(mAdapter.GetTextWithMaximumLength());

                    mTextPaint.TextSize = mTextSize * 1.0f;
                    return suggestedWith;
                }
                else
                {
                    var suggestedWith = (int)mTextPaint.MeasureText("0000");

                    mTextPaint.TextSize = mTextSize * 1.0f;
                    return suggestedWith;
                }
            }

            var widthForMinIndex = (int)mTextPaint.MeasureText(mMinIndex.ToString());
            var widthForMaxIndex = (int)mTextPaint.MeasureText(mMaxIndex.ToString());

            mTextPaint.TextSize = mTextSize * 1.0f;
            return widthForMinIndex > widthForMaxIndex ? widthForMinIndex : widthForMaxIndex;
        }

        private int CalculateSize(int suggestedSize, int paramSize, int measureSpec)
        {
            var result = 0;
            var size = MeasureSpec.GetSize(measureSpec);
            var mode = MeasureSpec.GetMode(measureSpec);

            switch (MeasureSpec.GetMode((int)mode))
            {
                case MeasureSpecMode.AtMost:
                    if (paramSize == ViewGroup.LayoutParams.WrapContent)
                    {
                        result = Math.Min(suggestedSize, size);
                    }
                    else if (paramSize == ViewGroup.LayoutParams.MatchParent)
                    {
                        result = size;
                    }
                    else
                    {
                        result = Math.Min(paramSize, size);
                    }
                    break;

                case MeasureSpecMode.Exactly:
                    result = size;
                    break;

                case MeasureSpecMode.Unspecified:
                    if (paramSize == ViewGroup.LayoutParams.WrapContent
                        || paramSize == ViewGroup.LayoutParams.MatchParent)
                    {
                        result = suggestedSize;
                    }
                    else
                    {
                        result = paramSize;
                    }
                    break;
            }
            return result;
        }

        private void InitializeSelectorWheel()
        {
            mItemHeight = GetItemHeight();
            mTextHeight = ComputeTextHeight();
            mTextGapHeight = GetGapHeight();

            var visibleMiddleItemPos = mItemHeight * mWheelVisibleItemMiddleIndex + (mItemHeight + mTextHeight) / 2;
            mInitialFirstItemOffset = visibleMiddleItemPos - mItemHeight * mWheelMiddleItemIndex;
            mCurrentFirstItemOffset = mInitialFirstItemOffset;

            mRect.Set(PaddingLeft, (Height / 2) - (mItemHeight / 2), Width - PaddingRight, (Height / 2) + (mItemHeight / 2));
        }

        private void InitializeFadingEdges()
        {
            VerticalFadingEdgeEnabled = mFadingEdgeEnabled;

            if (mFadingEdgeEnabled)
            {
                SetFadingEdgeLength((Bottom - Top - mTextSize) / 2);
            }
        }

        private void InitializeSelectorWheelIndices()
        {
            mSelectorItemIndices.Clear();
            mSelectorItemValidStatus.Clear();

            if (mMinValidIndex == null || mMinValidIndex < mMinIndex)
            {
                mCurrentSelectedItemIndex = mMinIndex <= 0 ? 0 : mMinIndex;
            }
            else
            {
                mCurrentSelectedItemIndex = mMinValidIndex <= 0 ? 0 : mMinValidIndex.Value;
            }

            for (int i = 0; i < mSelectorItemCount; i++)
            {
                var selectorIndex = mCurrentSelectedItemIndex + (i - mWheelMiddleItemIndex);

                if (mWrapSelectorWheelPreferred)
                {
                    selectorIndex = GetWrappedSelectorIndex(selectorIndex);
                }

                mSelectorItemIndices.Add(selectorIndex);
                mSelectorItemValidStatus.Add(IsValidPosition(selectorIndex));
            }
        }

        protected override float BottomFadingEdgeStrength => TopBottomFadingEdgeStrength;
        protected override float TopFadingEdgeStrength => TopBottomFadingEdgeStrength;

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawVertical(canvas);
        }

        private void DrawVertical(Canvas canvas)
        {
            if (mSelectorItemIndices.Count == 0)
            {
                return;
            }

            int i = 0;
            int x = canvas.Width / 2;
            int y = mCurrentFirstItemOffset;
            var itemHeight = GetItemHeight();

            switch (mTextAlign)
            {
                case WheelItemAlign.Left:
                    mTextPaint.TextAlign = Paint.Align.Left;
                    break;
                case WheelItemAlign.Center:
                    mTextPaint.TextAlign = Paint.Align.Center;
                    break;
                case WheelItemAlign.Right:
                    mTextPaint.TextAlign = Paint.Align.Right;
                    break;
            }

            var topIndexDiffToMid = mWheelVisibleItemMiddleIndex;
            var bottomIndexDiffToMid = mSelectorVisibleItemCount - mWheelVisibleItemMiddleIndex - 1;
            var maxIndexDiffToMid = Math.Max(topIndexDiffToMid, bottomIndexDiffToMid);

            while (i < mSelectorItemIndices.Count)
            {
                var scale = 1f;
                var offsetToMiddle = Math.Abs(y - (mInitialFirstItemOffset + mWheelMiddleItemIndex * itemHeight));

                if (maxIndexDiffToMid != 0)
                {
                    scale = mSelectedTextScale * (itemHeight * maxIndexDiffToMid - offsetToMiddle) / (itemHeight * maxIndexDiffToMid) + 1;
                }

                if (mSelectorItemValidStatus[i])
                {
                    if (offsetToMiddle < mItemHeight / 2)
                    {
                        mTextPaint.Color = mSelectedBackgroundColor;
                        mTextPaint.SetStyle(Paint.Style.Fill);
                        canvas.DrawRect(mRect, mTextPaint);

                        if (mSelectedTypeface != null)
                        {
                            mTextPaint.SetTypeface(mSelectedTypeface);
                        }
                        mTextPaint.Color = mSelectedTextColor;
                    }
                    else
                    {
                        if (mTypeface != null)
                        {
                            mTextPaint.SetTypeface(mTypeface);
                        }
                        mTextPaint.Color = mTextColor;
                    }
                }
                else
                {
                    if (mTypeface != null)
                    {
                        mTextPaint.SetTypeface(mTypeface);
                    }
                    mTextPaint.Color = Color.ParseColor("#E0E0E0");
                }

                canvas.Save();
                canvas.Scale(scale, scale, x, y);
                canvas.DrawText(GetValue(mSelectorItemIndices[i]), x, y, mTextPaint);
                canvas.Restore();

                y += itemHeight;
                i++;
            }
        }

        private int GetPosition(string value)
        {
            if (mAdapter != null)
            {
                return ValidatePosition(mAdapter.GetPosition(value));
            }
            else
            {
                try
                {
                    var position = value.ToInt();
                    return ValidatePosition(position);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return 0;
                }
            }
        }

        private void IncreaseSelectorsIndex()
        {
            for (int i = 0; i < mSelectorItemIndices.Count - 1; i++)
            {
                mSelectorItemIndices[i] = mSelectorItemIndices[i + 1];
                mSelectorItemValidStatus[i] = mSelectorItemValidStatus[i + 1];
            }

            var nextScrollSelectorIndex = mSelectorItemIndices[mSelectorItemIndices.Count - 2] + 1;

            if (mWrapSelectorWheelPreferred && nextScrollSelectorIndex > mMaxIndex)
            {
                nextScrollSelectorIndex = mMinIndex;
            }
            mSelectorItemIndices[mSelectorItemIndices.Count - 1] = nextScrollSelectorIndex;
            mSelectorItemValidStatus[mSelectorItemIndices.Count - 1] = IsValidPosition(nextScrollSelectorIndex);
        }

        private void DecreaseSelectorsIndex()
        {
            for (int i = mSelectorItemIndices.Count - 1; i > 0; i--)
            {
                mSelectorItemIndices[i] = mSelectorItemIndices[i - 1];
                mSelectorItemValidStatus[i] = mSelectorItemValidStatus[i - 1];
            }

            var nextScrollSelectorIndex = mSelectorItemIndices[1] - 1;

            if (mWrapSelectorWheelPreferred && nextScrollSelectorIndex < mMinIndex)
            {
                nextScrollSelectorIndex = mMaxIndex;
            }
            mSelectorItemIndices[0] = nextScrollSelectorIndex;
            mSelectorItemValidStatus[0] = IsValidPosition(nextScrollSelectorIndex);
        }

        private void ChangeValueBySteps(int steps)
        {
            mPreviousScrollerY = 0;
            mOverScroller?.StartScroll(0, 0, 0, -mItemHeight * steps, SnapScrollDuration);
            Invalidate();
        }

        private void OnSelectionChanged(int current, bool notifyChange)
        {
            var previous = mCurrentSelectedItemIndex;
            mCurrentSelectedItemIndex = current;

            if (notifyChange && previous != current)
            {
                NotifyChange(previous, current);
            }
        }

        private void NotifyChange(int previous, int current)
        {
            mOnValueChangeListener?.OnValueChange(this, GetValue(previous), GetValue(current));
        }

        private int ValidatePosition(int position)
        {
            if (!mWrapSelectorWheelPreferred)
            {
                if (mMaxValidIndex == null && position > mMaxIndex)
                {
                    return mMaxIndex;
                }
                else if (mMaxValidIndex != null && position > mMaxValidIndex)
                {
                    return mMaxValidIndex.Value;
                }
                else if (mMinValidIndex == null && position < mMinIndex)
                {
                    return mMinIndex;

                }
                else if (mMinValidIndex != null && position < mMinValidIndex)
                {
                    return mMinValidIndex.Value;
                }
                else
                {
                    return position;
                }
            }
            else
            {
                return GetWrappedSelectorIndex(position);
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            OnTouchEventVertical(e);
            return true;
        }

        private void OnTouchEventVertical(MotionEvent e)
        {
            if (mVelocityTracker == null)
            {
                mVelocityTracker = VelocityTracker.Obtain();
            }
            mVelocityTracker?.AddMovement(e);

            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    if (!mOverScroller.IsFinished)
                    {
                        mOverScroller.ForceFinished(true);
                    }

                    mLastY = e.GetY();
                    break;

                case MotionEventActions.Move:
                    var deltaY = e.GetY() - mLastY;

                    if (!mIsDragging && Math.Abs(deltaY) > mTouchSlop)
                    {
                        Parent?.RequestDisallowInterceptTouchEvent(true);

                        if (deltaY > 0)
                        {
                            deltaY -= mTouchSlop;
                        }
                        else
                        {
                            deltaY += mTouchSlop;
                        }

                        OnScrollStateChange(WheelScrollState.TouchScroll);
                        mIsDragging = true;
                    }

                    if (mIsDragging)
                    {
                        ScrollBy(0, (int)deltaY);
                        Invalidate();
                        mLastY = e.GetY();
                    }
                    break;

                case MotionEventActions.Up:
                    if (mIsDragging)
                    {
                        mIsDragging = false;
                        Parent?.RequestDisallowInterceptTouchEvent(false);

                        mVelocityTracker?.ComputeCurrentVelocity(1000, mMaxVelocity);
                        var velocity = (int)mVelocityTracker?.YVelocity;

                        if (Math.Abs(velocity) > mMinVelocity)
                        {
                            mPreviousScrollerY = 0;

                            mOverScroller?.Fling(
                                ScrollX, ScrollY, 0, velocity, 0, 0, int.MinValue,
                                int.MaxValue, 0, (int)(GetItemHeight() * 0.7)
                            );

                            InvalidateOnAnimation();
                            OnScrollStateChange(WheelScrollState.Fling);
                        }
                        RecyclerVelocityTracker();
                    }
                    else
                    {
                        //click event
                        HandlerClickVertical((int)e.GetY());
                    }
                    break;

                case MotionEventActions.Cancel:
                    if (mIsDragging)
                    {
                        mIsDragging = false;
                    }
                    RecyclerVelocityTracker();
                    break;
            }
        }

        private void HandlerClickVertical(int y)
        {
            var selectorIndexOffset = y / mItemHeight - mWheelVisibleItemMiddleIndex;
            ChangeValueBySteps(selectorIndexOffset);
        }

        public override void ScrollBy(int x, int y)
        {
            if (y == 0)
            {
                return;
            }

            var gap = mTextGapHeight;

            if (!mWrapSelectorWheelPreferred && y > 0
                && (mSelectorItemIndices[mWheelMiddleItemIndex] <= mMinIndex
                || (mSelectorItemIndices[mWheelMiddleItemIndex] <= mMinValidIndex)))
            {
                if (mCurrentFirstItemOffset + y - mInitialFirstItemOffset < gap / 2)
                {
                    mCurrentFirstItemOffset += y;
                }
                else
                {
                    mCurrentFirstItemOffset = mInitialFirstItemOffset + (gap / 2);

                    if (!mOverScroller.IsFinished && !mIsDragging)
                    {
                        mOverScroller.AbortAnimation();
                    }
                }
                return;
            }

            if (!mWrapSelectorWheelPreferred && y < 0
                && (mSelectorItemIndices[mWheelMiddleItemIndex] >= mMaxIndex
                || (mSelectorItemIndices[mWheelMiddleItemIndex] >= mMaxValidIndex)))
            {
                if (mCurrentFirstItemOffset + y - mInitialFirstItemOffset > -(gap / 2))
                {
                    mCurrentFirstItemOffset += y;
                }
                else
                {
                    mCurrentFirstItemOffset = mInitialFirstItemOffset - (gap / 2);

                    if (!mOverScroller.IsFinished && !mIsDragging)
                    {
                        mOverScroller.AbortAnimation();
                    }
                }
                return;
            }

            mCurrentFirstItemOffset += y;

            while (mCurrentFirstItemOffset - mInitialFirstItemOffset < -gap)
            {
                mCurrentFirstItemOffset += mItemHeight;
                IncreaseSelectorsIndex();

                if (!mWrapSelectorWheelPreferred
                    && (mSelectorItemIndices[mWheelMiddleItemIndex] >= mMaxIndex
                    || (mSelectorItemIndices[mWheelMiddleItemIndex] >= mMaxValidIndex)))
                {
                    mCurrentFirstItemOffset = mInitialFirstItemOffset;
                }
            }

            while (mCurrentFirstItemOffset - mInitialFirstItemOffset > gap)
            {
                mCurrentFirstItemOffset -= mItemHeight;
                DecreaseSelectorsIndex();

                if (!mWrapSelectorWheelPreferred
                    && (mSelectorItemIndices[mWheelMiddleItemIndex] <= mMinIndex
                    || (mSelectorItemIndices[mWheelMiddleItemIndex] <= mMinValidIndex)))
                {
                    mCurrentFirstItemOffset = mInitialFirstItemOffset;
                }
            }
            OnSelectionChanged(mSelectorItemIndices[mWheelMiddleItemIndex], true);
        }

        public override void ComputeScroll()
        {
            base.ComputeScroll();
            if (mOverScroller.ComputeScrollOffset())
            {
                var x = mOverScroller.CurrX;
                var y = mOverScroller.CurrY;

                if (mPreviousScrollerY == 0)
                {
                    mPreviousScrollerY = mOverScroller.StartY;
                }

                ScrollBy(x, y - mPreviousScrollerY);
                mPreviousScrollerY = y;
                Invalidate();
            }
            else
            {
                if (!mIsDragging)
                {
                    //align item
                    AdjustItemVertical();
                }
            }
        }

        private void AdjustItemVertical()
        {
            mPreviousScrollerY = 0;
            var deltaY = mInitialFirstItemOffset - mCurrentFirstItemOffset;

            if (Math.Abs(deltaY) > mItemHeight / 2)
            {
                deltaY += deltaY > 0 ? -mItemHeight : mItemHeight;
            }

            if (deltaY != 0)
            {
                mOverScroller.StartScroll(ScrollX, ScrollY, 0, deltaY, 800);
                InvalidateOnAnimation();
            }

            OnScrollStateChange(WheelScrollState.Idle);
        }

        private void RecyclerVelocityTracker()
        {
            mVelocityTracker?.Recycle();
            mVelocityTracker = null;
        }

        private void OnScrollStateChange(WheelScrollState scrollState)
        {
            if (mScrollState == scrollState)
            {
                return;
            }

            mScrollState = scrollState;
            mOnScrollListener?.OnScrollStateChange(this, scrollState);
        }

        private bool IsValidPosition(int selectorIndex)
        {
            if (selectorIndex < mMinValidIndex)
            {
                return false;
            }
            else if (selectorIndex > mMaxValidIndex)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int GetWrappedSelectorIndex(int selectorIndex)
        {
            if (selectorIndex > mMaxIndex)
            {
                return mMinIndex + (selectorIndex - mMaxIndex) % (mMaxIndex - mMinIndex + 1) - 1;
            }
            else if (selectorIndex < mMinIndex)
            {
                return mMaxIndex - (mMinIndex - selectorIndex) % (mMaxIndex - mMinIndex + 1) + 1;
            }
            return selectorIndex;
        }

        private int GetItemHeight()
        {
            return Height / (mSelectorItemCount - 2);
        }

        private int GetGapHeight()
        {
            return GetItemHeight() - ComputeTextHeight();
        }

        private int ComputeTextHeight()
        {
            var metricsInt = mTextPaint.GetFontMetricsInt();
            return Math.Abs(metricsInt.Bottom + metricsInt.Top);
        }

        private void InvalidateOnAnimation()
        {
            Invalidate();
        }

        public void ScrollTo(int position)
        {
            if (mCurrentSelectedItemIndex == position)
            {
                return;
            }

            mCurrentSelectedItemIndex = position;
            mSelectorItemIndices.Clear();

            for (int i = 0; i < mSelectorItemCount; i++)
            {
                var selectorIndex = mCurrentSelectedItemIndex + (i - mWheelMiddleItemIndex);

                if (mWrapSelectorWheelPreferred)
                {
                    selectorIndex = GetWrappedSelectorIndex(selectorIndex);
                }
                mSelectorItemIndices.Add(selectorIndex);
            }
            Invalidate();
        }

        public void SetOnValueChangedListener(IOnValueChangeListener listener)
        {
            mOnValueChangeListener = listener;
        }

        public void SetOnScrollListener(IOnScrollListener listener)
        {
            mOnScrollListener = listener;
        }

        public void SmoothScrollTo(int position)
        {
            var realPosition = ValidatePosition(position);
            ChangeValueBySteps(realPosition - mCurrentSelectedItemIndex);
        }

        public void SmoothScrollToValue(string value)
        {
            SmoothScrollTo(GetPosition(value));
        }

        public void ScrollToValue(string value)
        {
            ScrollTo(GetPosition(value));
        }

        public void SetAdapter(WheelAdapter adapter, bool indexRangeBasedOnAdapterSize = true)
        {
            mAdapter = adapter;

            if (mAdapter == null)
            {
                InitializeSelectorWheelIndices();
                Invalidate();
                return;
            }

            if (adapter.GetSize() != -1 && indexRangeBasedOnAdapterSize)
            {
                mMaxIndex = adapter.GetSize() - 1;
                mMinIndex = 0;
            }

            mMaxValidIndex = adapter.GetMaxValidIndex();
            mMinValidIndex = adapter.GetMinValidIndex();
            InitializeSelectorWheelIndices();
            Invalidate();
            mAdapter.Picker = this;
        }

        public void SetWrapSelectorWheel(bool wrap)
        {
            mWrapSelectorWheelPreferred = wrap;
            Invalidate();
        }

        public void SetWheelItemCount(int count)
        {
            mSelectorItemCount = count + 2;
            mWheelMiddleItemIndex = (mSelectorItemCount - 1) / 2;

            mSelectorVisibleItemCount = mSelectorItemCount - 2;
            mWheelVisibleItemMiddleIndex = (mSelectorVisibleItemCount - 1) / 2;

            mSelectorItemIndices = new List<int>(mSelectorItemCount);
            mSelectorItemValidStatus = new List<bool>(mSelectorItemCount);

            Reset();
            Invalidate();
        }

        public string GetValue(int position)
        {
            if (mAdapter != null)
            {
                return mAdapter.GetValue(position);
            }
            else
            {
                if (!mWrapSelectorWheelPreferred)
                {
                    if (position > mMaxIndex || position < mMinIndex)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return position.ToString();
                    }
                }
                else
                {
                    return GetWrappedSelectorIndex(position).ToString();
                }
            }
        }

        public void SetValue(string value)
        {
            ScrollToValue(value);
        }

        public void SetMaxValue(int max)
        {
            mMaxIndex = max;
        }

        public string GetMaxValue()
        {
            if (mAdapter != null)
            {
                return mAdapter.GetValue(mMaxIndex);
            }
            else
            {
                return mMaxIndex.ToString();
            }
        }

        public void SetMinValue(int min)
        {
            mMinIndex = min;
        }

        public string GetMinValue()
        {
            if (mAdapter != null)
            {
                return mAdapter.GetValue(mMinIndex);
            }
            else
            {
                return mMinIndex.ToString();
            }
        }

        public void SetMinValidValue(int? minValid)
        {
            mMinValidIndex = minValid;
        }

        public void SetMaxValidValue(int? maxValid)
        {
            mMaxValidIndex = maxValid;
        }

        public void Reset()
        {
            InitializeSelectorWheelIndices();
            InitializeSelectorWheel();
            Invalidate();
        }

        public string GetCurrentItem()
        {
            return GetValue(mCurrentSelectedItemIndex);
        }

        public int GetCurrentIndex()
        {
            return mCurrentSelectedItemIndex;
        }

        public void SetSelectedItemBackgroundColor(Color color)
        {
            mSelectedBackgroundColor = color;
            Invalidate();
        }

        public void SetSelectedTextColor(Color color)
        {
            mSelectedTextColor = color;
            Invalidate();
        }

        public void SetTextColor(Color color)
        {
            mTextColor = color;
            Invalidate();
        }

        public void SetTextSize(int size)
        {
            mTextSize = size;
            Invalidate();
        }

        public void SetTextAlign(WheelItemAlign itemAlign)
        {
            mTextAlign = itemAlign;
            Invalidate();
        }

        public void SetTypeface(Typeface typeface)
        {
            mTypeface = typeface;
        }

        public void SetSelectedTypeface(Typeface typeface)
        {
            mSelectedTypeface = typeface;
        }
    }

    public enum WheelScrollState
    {
        Idle = 0,
        TouchScroll = 1,
        Fling = 2
    }

    public enum WheelItemAlign
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public interface IOnValueChangeListener
    {
        void OnValueChange(WheelPicker picker, string oldValue, string newValue);
    }

    public interface IOnScrollListener
    {
        void OnScrollStateChange(WheelPicker picker, WheelScrollState state);
    }
}

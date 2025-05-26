namespace MAUIEssentials.AppCode.Helpers
{
    public enum GradientOrientation
    {
        Vertical,
        Horizontal,
        Diagonal,
        ReverseDiagonal
    }

    public enum LocalBioMetricOption
    {
        None,
        Fingerprint,
        Face,
    }

    public enum NotificationType
    {
        Error,
        Failed,
        Info,
        Success,
    }

    public enum TooltipPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum GeoCodeCalcMeasurement : int
    {
        Miles = 0,
        Kilometers = 1
    }

    public enum AppTypeEnum
    {
        Customer = 1,
        Owner = 2
    }
}
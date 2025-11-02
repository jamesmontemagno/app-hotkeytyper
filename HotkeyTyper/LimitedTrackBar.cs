using System.Drawing.Drawing2D;

namespace HotkeyTyper;

/// <summary>
/// Custom track bar that supports a soft maximum (SoftMax) and greys out ticks above it.
/// Owner-drawn to provide visual affordance when code mode limits speed.
/// </summary>
internal class LimitedTrackBar : TrackBar
{
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int? SoftMax { get; set; }

    public LimitedTrackBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        TickStyle = TickStyle.None; // We'll draw ticks manually
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = ClientRectangle;
        int left = 8;
        int right = rect.Width - 8;
        int trackY = rect.Height / 2;

        using (var trackPen = new Pen(AppColors.TrackBarTrack, 3))
        {
            e.Graphics.DrawLine(trackPen, left, trackY, right, trackY);
        }

        // Draw ticks
        int range = Maximum - Minimum;
        for (int v = Minimum; v <= Maximum; v++)
        {
            float ratio = range == 0 ? 0 : (float)(v - Minimum) / range;
            int x = left + (int)((right - left) * ratio);
            bool disabled = SoftMax.HasValue && v > SoftMax.Value;
            using var tickPen = new Pen(disabled ? AppColors.TrackBarTickDisabled : AppColors.TrackBarTick, 1);
            e.Graphics.DrawLine(tickPen, x, trackY - 6, x, trackY + 6);
        }

        // Draw thumb
        float thumbRatio = range == 0 ? 0 : (float)(Value - Minimum) / range;
        int thumbX = left + (int)((right - left) * thumbRatio);
        var thumbRect = new Rectangle(thumbX - 7, trackY - 11, 14, 22);
        using (var thumbBrush = new SolidBrush(AppColors.TrackBarThumb))
            e.Graphics.FillEllipse(thumbBrush, thumbRect);
        using (var outlinePen = new Pen(AppColors.TrackBarThumbOutline, 1))
            e.Graphics.DrawEllipse(outlinePen, thumbRect);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        SetValueFromMouse(e.X);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button == MouseButtons.Left)
        {
            SetValueFromMouse(e.X);
        }
    }

    private void SetValueFromMouse(int mouseX)
    {
        int left = 8;
        int right = Width - 8;
        int clamped = Math.Min(Math.Max(mouseX, left), right);
        float ratio = (float)(clamped - left) / (right - left);
        int newVal = Minimum + (int)Math.Round(ratio * (Maximum - Minimum));
        if (SoftMax.HasValue && newVal > SoftMax.Value)
        {
            newVal = SoftMax.Value;
        }
        if (newVal != Value)
        {
            Value = newVal;
            Invalidate();
            OnValueChanged(EventArgs.Empty);
        }
    }
}

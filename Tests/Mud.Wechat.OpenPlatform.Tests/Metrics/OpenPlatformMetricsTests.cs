// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.Metrics;
using Mud.Wechat.OpenPlatform.Abstractions.Metrics;

namespace Mud.Wechat.OpenPlatform.Tests.Metrics;

/// <summary>
/// 开放平台指标面测试（总案 D5）：MeterListener 实采断言计数/直方图按产品线标签落点。
/// </summary>
public class OpenPlatformMetricsTests
{
    [Fact]
    public void Metrics_ShouldRecordWithOpenPlatformProductTag_WhenHelperInvoked()
    {
        var recorded = new List<(string Instrument, double Value, Dictionary<string, object?> Tags)>();
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, enable) =>
        {
            if (instrument.Meter.Name == OpenPlatformMetrics.MeterName)
            {
                enable.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
            recorded.Add((instrument.Name, value, ToTags(tags))));
        listener.SetMeasurementEventCallback<double>((instrument, value, tags, _) =>
            recorded.Add((instrument.Name, value, ToTags(tags))));
        listener.Start();

        using (OpenPlatformMetricsHelper.RecordCallbackRequest("op_test"))
        {
        }

        OpenPlatformMetricsHelper.RecordCallbackOutcome("op_test", "failure", "verify");
        using (OpenPlatformMetricsHelper.RecordEventHandling("op_test", "component_verify_ticket"))
        {
        }

        OpenPlatformMetricsHelper.RecordEventOutcome("op_test", "component_verify_ticket", success: true);

        recorded.Should().Contain(r => r.Instrument == "callback.request.duration"
            && (string?)r.Tags["wechat.product"] == "openplatform",
            "开放平台产品线标签恒为 openplatform");
        recorded.Should().Contain(r => r.Instrument == "callback.request" && r.Value == 1
            && (string?)r.Tags["outcome"] == "failure"
            && (string?)r.Tags["error.type"] == "verify");
        recorded.Should().Contain(r => r.Instrument == "event.handling.duration"
            && (string?)r.Tags["wechat.op.event_type"] == "component_verify_ticket");
        recorded.Should().Contain(r => r.Instrument == "event.handling" && r.Value == 1
            && (string?)r.Tags["outcome"] == "success");
    }

    [Fact]
    public void MeterName_ShouldFollowProductLineConvention()
    {
        OpenPlatformMetrics.MeterName.Should().Be("Mud.Wechat.OpenPlatform");
    }

    /// <summary>TagList → 字典（回调携带 ReadOnlySpan，手写拷贝）。</summary>
    private static Dictionary<string, object?> ToTags(ReadOnlySpan<KeyValuePair<string, object?>> tags)
    {
        var dict = new Dictionary<string, object?>(tags.Length);
        foreach (var tag in tags)
        {
            dict[tag.Key] = tag.Value;
        }

        return dict;
    }
}

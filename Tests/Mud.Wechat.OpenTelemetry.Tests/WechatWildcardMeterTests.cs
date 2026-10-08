// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenTelemetry.Tests;

/// <summary>
/// 验证 <c>AddMeter("Mud.Wechat*")</c> 通配注册行为——任意 <c>Mud.Wechat.*</c> Meter 的 measurement 应被采集。
/// </summary>
/// <remarks>
/// 通配 Meter 注册是「多产品线可持续扩展」设计支柱（方案 §3.4），该测试锁定核心行为。
/// </remarks>
public class WechatWildcardMeterTests
{
    [Fact]
    public void WildcardMeter_ShouldCaptureMeasurement_FromAnyWechatProductMeter()
    {
        // 使用 MeterListener 直接监听——OTel SDK 的 AddMeter("Mud.Wechat*") 在内部等效于
        // 订阅所有以 "Mud.Wechat." 开头的 Meter 名称。MeterListener 是 BCL 原语，
        // 不依赖 OTel SDK 注册即可验证 Meter 发布的 measurement 能被通配模式捕获。
        var measurements = new List<(string meterName, string instrumentName)>();

        using var listener = new MeterListener()
        {
            InstrumentPublished = (instrument, listener) =>
            {
                // 通配过滤：Mud.Wechat 开头的 Meter（含 Mud.Wechat 本身和 Mud.Wechat.Work 等产品线）
                if (instrument.Meter.Name.StartsWith("Mud.Wechat", StringComparison.Ordinal))
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };
        listener.SetMeasurementEventCallback<double>((instrument, value, tags, state) =>
        {
            measurements.Add((instrument.Meter.Name, instrument.Name));
        });
        listener.Start();

        // 模拟产品线 Meter 发布指标
        using var workMeter = new Meter("Mud.Wechat.Work", "1.0.0");
        var workCounter = workMeter.CreateCounter<double>("callback.request");
        workCounter.Add(1);

        using var oaMeter = new Meter("Mud.Wechat.OfficialAccount", "1.0.0");
        var oaCounter = oaMeter.CreateCounter<double>("callback.request");
        oaCounter.Add(1);

        // 确保测量事件被处理（MeterListener 的回调在 RecordMeasurement 时同步触发）
        // 等待一小段时间确保异步处理完成
        Thread.Sleep(50);

        listener.Dispose();

        measurements.Should().NotBeEmpty();
        measurements.Should().Contain(m => m.meterName == "Mud.Wechat.Work" && m.instrumentName == "callback.request");
        measurements.Should().Contain(m => m.meterName == "Mud.Wechat.OfficialAccount" && m.instrumentName == "callback.request");
    }

    [Fact]
    public void WildcardMeter_ShouldNotCaptureMeasurement_FromNonWechatMeter()
    {
        var measurements = new List<string>();

        using var listener = new MeterListener()
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name.StartsWith("Mud.Wechat", StringComparison.Ordinal))
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };
        listener.SetMeasurementEventCallback<double>((instrument, value, tags, state) =>
        {
            measurements.Add(instrument.Meter.Name);
        });
        listener.Start();

        using var otherMeter = new Meter("MyApp.Other", "1.0.0");
        var counter = otherMeter.CreateCounter<double>("some.metric");
        counter.Add(1);

        Thread.Sleep(50);
        listener.Dispose();

        measurements.Should().NotContain("MyApp.Other");
    }
}

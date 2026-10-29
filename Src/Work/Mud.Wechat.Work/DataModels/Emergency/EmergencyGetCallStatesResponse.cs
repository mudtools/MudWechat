// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Emergency;

/// <summary>
/// 获取接听状态响应体（<c>/cgi-bin/pstncc/getstates</c>，紧急通知域）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Emergency")]
public class EmergencyGetCallStatesResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否接听：0 表示未接听，1 表示接听。</summary>
    [JsonPropertyName("istalked")]
    public int? Istalked { get; set; }

    /// <summary>获取或设置呼叫发起时间戳。</summary>
    [JsonPropertyName("calltime")]
    public long? Calltime { get; set; }

    /// <summary>获取或设置通话时长，单位秒。</summary>
    [JsonPropertyName("talktime")]
    public int? Talktime { get; set; }

    /// <summary>
    /// 获取或设置呼叫结果状态（0 正常结束；1~17、20、99 为各类异常 / 中间状态）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方取值口径：0 正常结束；1 振铃；2 接听；3 通话中；4 呼叫超时-用户挂机；5 不在服务区；
    /// 6 欠费未接听；7 被叫拒接；8 被叫关机；9 空号；10 呼叫受限；11 线路错误；12 呼叫超时-系统挂机；
    /// 13 呼叫超过限制（8 分钟 3 次 24 小时 8 次）；14 线路超时未返回；15 超限（主叫超限，需要换号码呼叫）；
    /// 16 线路繁忙-稍后在呼；17 呼叫取消通知；20 外呼超时未确认；99 其他。
    /// </para>
    /// </remarks>
    [JsonPropertyName("reason")]
    public int? Reason { get; set; }
}

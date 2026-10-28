// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 付款礼品卡规则基础信息（官方 <c>base_info</c>；add 与 getbyid 共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftBaseInfo
{
    /// <summary>获取或设置适用商户号列表（官方 <c>mchid_list</c>，必填）。</summary>
    [JsonPropertyName("mchid_list")]
    public List<string> MchIdList { get; set; } = new();

    /// <summary>获取或设置生效时间（官方 <c>begin_time</c>，秒级 Unix 时间戳，必填）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>获取或设置失效时间（官方 <c>end_time</c>，秒级 Unix 时间戳，必填）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置规则状态（官方 <c>status</c>；仅 getbyid 响应返回）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>；仅 getbyid 响应返回）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>；仅 getbyid 响应返回）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}

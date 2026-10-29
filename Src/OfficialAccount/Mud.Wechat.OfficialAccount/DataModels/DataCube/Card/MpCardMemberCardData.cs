// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 会员卡统计单日数据项（官方 <c>getcardmembercardinfo</c> 的 <c>list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardData
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>浏览次数（官方 <c>view_cnt</c>）。</summary>
    [JsonPropertyName("view_cnt")]
    public int ViewCount { get; set; }

    /// <summary>浏览人数（官方 <c>view_user</c>）。</summary>
    [JsonPropertyName("view_user")]
    public int ViewUserCount { get; set; }

    /// <summary>领取次数（官方 <c>receive_cnt</c>）。</summary>
    [JsonPropertyName("receive_cnt")]
    public int ReceiveCount { get; set; }

    /// <summary>领取人数（官方 <c>receive_user</c>）。</summary>
    [JsonPropertyName("receive_user")]
    public int ReceiveUserCount { get; set; }

    /// <summary>核销次数（官方 <c>verify_cnt</c>）。</summary>
    [JsonPropertyName("verify_cnt")]
    public int ConsumeCount { get; set; }

    /// <summary>核销人数（官方 <c>verify_user</c>）。</summary>
    [JsonPropertyName("verify_user")]
    public int ConsumeUserCount { get; set; }

    /// <summary>激活次数（官方 <c>active_cnt</c>）。</summary>
    [JsonPropertyName("active_cnt")]
    public int ActiveCount { get; set; }

    /// <summary>激活人数（官方 <c>active_user</c>）。</summary>
    [JsonPropertyName("active_user")]
    public int ActiveUserCount { get; set; }

    /// <summary>历史领取会员卡总人数（官方 <c>total_user</c>）。</summary>
    [JsonPropertyName("total_user")]
    public int TotalUserCount { get; set; }

    /// <summary>历史有效会员卡总人数（官方 <c>total_receive_user</c>）。</summary>
    [JsonPropertyName("total_receive_user")]
    public int TotalReceiveUserCount { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>用户增减数据条目（官方 <c>getusersummary</c> 的 list 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserSummaryItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置用户的渠道（官方 <c>user_source</c>：0 其他合计 / 1 公众号搜索 / 17 名片分享 / 30 扫描二维码 / 57 文章内账号名称 / 100 微信广告 / 161 他人转载 / 149 小程序关注 / 200 视频号 / 201 直播）。</summary>
    [JsonPropertyName("user_source")]
    public int? UserSource { get; set; }

    /// <summary>获取或设置新增的用户数量（官方 <c>new_user</c>）。</summary>
    [JsonPropertyName("new_user")]
    public int? NewUser { get; set; }

    /// <summary>获取或设置取消关注的用户数量（官方 <c>cancel_user</c>；new_user − cancel_user 即净增）。</summary>
    [JsonPropertyName("cancel_user")]
    public int? CancelUser { get; set; }
}

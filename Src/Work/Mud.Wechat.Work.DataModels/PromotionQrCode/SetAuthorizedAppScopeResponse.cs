// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 设置授权应用可见范围响应体（<c>/cgi-bin/agent/set_scope</c>）。
/// </summary>
/// <remarks>
/// 官方返回体除 <c>errcode</c>/<c>errmsg</c> 外还给出三类<b>非法项</b>列表
/// （可见范围中不存在的成员 / 部门 / 标签），因此属「有业务负载」响应，不得退化为 <see cref="WechatWorkResponse"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class SetAuthorizedAppScopeResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置非法成员列表（可见范围中不存在的成员 userid）。
    /// </summary>
    [JsonPropertyName("invaliduser")]
    public List<string>? InvalidUser { get; set; }

    /// <summary>
    /// 获取或设置非法部门列表（可见范围中不存在的部门 ID）。
    /// </summary>
    [JsonPropertyName("invalidparty")]
    public List<int>? InvalidParty { get; set; }

    /// <summary>
    /// 获取或设置非法标签列表（可见范围中不存在的标签 ID）。
    /// </summary>
    [JsonPropertyName("invalidtag")]
    public List<int>? InvalidTag { get; set; }
}

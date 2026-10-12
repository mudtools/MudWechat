// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>获取类目（<c>wxaapi/newtmpl/getcategory</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；data 元素<b>仅 id/name 两字段</b>
/// （无 type——与任务预期不同，以页面为准）。页面描述「获取小程序、公众号所属类目」系小程序文案复用，照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeCategoryListResponse : MpResponse
{
    /// <summary>获取或设置类目列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribeCategory>? Data { get; set; }
}

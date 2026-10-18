// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.contact.get</c>：按 external_userid 读取客户详情与跟进关系。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影边界</b>：官方响应含客户微信昵称、头像、unionid、备注手机号，以及跟进成员的备注描述；
/// 执行器只回填「客户是谁（昵称/类型/公司/职位）+ 谁在跟进（成员 + 备注名 + 添加时间/方式 + 标签）」，
/// <b>不外发</b>头像、unionid、备注手机号、客户微信渠道昵称（个人信息一旦进入提示词与日志即不可撤回）。
/// </para>
/// <para>
/// <b>跟进关系可能分页</b>：官方 <c>get</c> 支持 <c>cursor</c> 续页，本工具只取首页并在返回里标注
/// <c>page_token</c> 是否存在；需要全量跟进人时由宿主扩展专用工具，不在本工具里静默丢弃。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.contact.get",
    Description = "按客户 ID 获取企业微信客户详情与跟进关系。参数 external_userid 为客户 ID（先用 externalcontact.contact.list 获取）。返回 external_userid、name（客户微信昵称）、type（1 微信用户 / 2 企业微信用户）、gender、corp_name（客户所属企业）、position（客户职位）、follow_users（跟进成员：userid、备注名、添加时间、添加方式、客户状态、标签名）与 follow_user_count。不含头像、unionid、备注手机号与视频号昵称（个人数据不外发）。若返回 follow_user_truncated 为 true，说明跟进成员被官方截断，本工具只回填首页（该端点无翻页把手）。",
    Source = "IWechatWorkExternalContactCustomerService.GetCustomerDetailAsync",
    RequiredScopes = ["externalcontact.contact.read"])]
public interface IWechatWorkExternalContactContactGetTool
{
    /// <summary>读取客户详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（external_userid、name、type、gender、corp_name、position、follow_users、follow_user_count）。</returns>
    Task<string> GetCustomerDetailAsync(
        [ToolParameter("external_userid", "客户 ID。", Required = true)] string external_userid,
        CancellationToken cancellationToken = default);
}

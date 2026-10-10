// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块「获客助手组件」域第三方应用 SDK：
/// 官方仅向第三方应用开放（企业自建应用与服务商代开发均无对应功能），本接口仅保留
/// 2 个组件专属端点（获取组件授权信息、生成代支付 key）。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，企业级令牌一企一份，
/// 宿主须以授权企业 scope 获取）。官方权限口径：获客助手组件可调用，组件版仅可获取 / 查询
/// <b>授权给获客助手组件</b>的链接。
/// </para>
/// <para>
/// 架构决策（单一所有者）：获客链接列表 / 详情 / 使用统计 / 成员收消息详情
/// （<c>list_link</c>、<c>get</c>、<c>statistic</c>、<c>get_chat_info</c>）曾在本接口重复声明——官方对
/// 「获客助手域」与「获客助手组件域」以<b>同一路由、两套文档</b>承载，差异仅在于组件版返回字段为
/// 自建版的可空真子集（组件版链接详情仅含 link_name / url、收消息详情不返回顶层 userid /
/// external_userid）。按 ADR-14「一份可空超集载荷覆盖多模式」原则，已删除本接口的这 4 处重复声明，
/// 统一收敛为「获客助手域」<see cref="IWechatWorkExternalContactCustomerAcquisitionService"/> 单一所有者。
/// 第三方组件应用请改用 <see cref="IWechatWorkThirdPartyExternalContactCustomerAcquisitionService"/>。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ExternalContact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkExternalContactAcquisitionComponentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyExternalContactAcquisitionComponentService : IWechatWorkExternalContactAcquisitionComponentService
{
    /// <summary>
    /// 获取组件授权信息
    /// <para>服务商可获取企业获客助手组件的授权信息：付费模式（扣企业 / 扣服务商）、
    /// 代付单价（pay_mode=1 时返回）、链接授权模式与授权的应用列表。</para>
    /// <para>官方契约本端点为 POST 且无请求体参数。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>组件授权信息（pay_mode / price / link_auth_mode / auth_apps）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99610"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info")]
    Task<GetAcquisitionComponentAuthInfoResponse> GetAcquisitionComponentAuthInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成代支付 key
    /// <para>为获客链接生成一次性代支付 key（14 天内有效）；使用方式为将 <c>once_key</c> 参数拼接到
    /// <c>comp_scene</c> 链接尾部。key_num 默认 100，最大不超过 1000。</para>
    /// <para>官方口径：需企业授权为「服务商代支付模式」（pay_mode=1）方可调用。</para>
    /// </summary>
    /// <param name="request">生成请求体（<see cref="CreateAcquisitionComponentOnceKeyRequest"/>：link_id / key_num）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代支付 key 列表（keys）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99603"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/create_once_key")]
    Task<CreateAcquisitionComponentOnceKeyResponse> CreateAcquisitionComponentOnceKeyAsync(
        [Body] CreateAcquisitionComponentOnceKeyRequest request,
        CancellationToken cancellationToken = default);
}

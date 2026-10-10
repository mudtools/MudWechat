// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「微信搜一搜」域 SDK（1 端点：搜一搜数据推送）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 微信搜一搜，2026-10-10 依据官方清单核验）：
/// <c>wxsearch/api_submitpages.html</c>（搜一搜数据推送）。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：小程序通过本接口推送<b>优质内容的页面路径、参数和结构化数据</b>，
/// 让微信搜索更及时地收录小程序内容；推送内容将用于微信搜索结果展示。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：走应用级 <c>access_token</c>（Query）。官方为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Search", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaSearchService
{
    /// <summary>
    /// 搜一搜数据推送。官方文档：<c>wxsearch/api_submitpages.html</c>。
    /// </summary>
    /// <param name="request">页面信息推送请求（<c>pages</c> 必填），见 <see cref="DataModels.Search.WxaSearchSubmitPagesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标准应答（<c>errcode</c> / <c>errmsg</c>），见 <see cref="DataModels.WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/search/wxaapi_submitpages</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>内容约束（官方原文）</b>：推送内容须为<b>优质内容</b>方有机会被微信搜索收录并展示；
    /// 结构化为 <c>data_list</c>（<c>@type</c> 逐条声明数据结构类型），SDK 不做本地校验。
    /// </para>
    /// </remarks>
    [Post("/wxa/search/wxaapi_submitpages")]
    Task<DataModels.WxaResponse> SubmitPagesAsync(
        [Body] DataModels.Search.WxaSearchSubmitPagesRequest request,
        CancellationToken cancellationToken = default);
}
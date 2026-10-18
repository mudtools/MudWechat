// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Service;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「服务商服务端」域「通讯录搜索 / 排序 / id 转译」接口族公共父接口（官方 91844 / 91846 / 92093）。
/// <para>
/// 本族四支端点全部位于 <c>/cgi-bin/service/contact/</c> 前缀下、统一以<b>服务商凭证</b>
/// <c>provider_access_token</c> 鉴权，并以请求体里的 <c>auth_corpid</c> 指定被操作企业
/// —— 这是「服务商代开发」服务端的固定形态，<b>不消费</b>授权企业自身的 <c>access_token</c>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 应用类型开放面：官方四篇文档正文的请求地址均为 <c>?provider_access_token=ACCESS_TOKEN</c>，
/// 且「通讯录搜索」「id 转译」「排序」只出现在<b>服务商代开发 / 第三方应用开发</b>的服务端文档树内
/// （企业自建应用无对应面，自建侧同类能力走 <c>/cgi-bin/user/authsucc</c> 等 <c>access_token</c> 端点），
/// 故本子接口族仅落 <see cref="IWechatWorkProviderServiceContactService"/> 一个应用类型子接口。
/// </para>
/// <para>
/// <b>与既有 <c>Contact</c> 域的分界</b>：<c>Interfaces/Contacts/</c> 族走应用令牌
/// <c>access_token</c>（Query 参数名 <c>access_token</c>）且按 <c>AppType</c> 二分归属域键；
/// 本族走服务商凭证、令牌类型为 <c>WechatTokenTypes.ProviderAccessToken</c>，
/// 二者路由前缀（<c>service/contact/</c> ↔ <c>contact/</c>、<c>user/</c>、<c>department/</c>）零交叠，
/// 故独立成 <c>Service</c> 域而非并入 <c>Contact</c> 注册组（并入会让同一注册组出现两种令牌类型）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkServiceContactService
{
    /// <summary>
    /// 通讯录搜索
    /// <para>服务商按「名称 / 拼音 / 英文名」关键词搜索授权企业的成员与部门，支持分页与精确匹配。</para>
    /// <para>
    /// <b>官方参数语义（勿「顺手补全」）</b>：<c>query_type</c> 为 1 用户 / 2 部门，<b>不填或填 0 代表同时查询两者</b>；
    /// <c>query_range</c> 仅当查询类型包含用户时有效（0 只在职、1 在职 + 离职，离职用户仅当离职前激活过企业微信才可被搜到）；
    /// <c>agentid</c> 若非 0 则只返回该应用<b>可见范围内</b>的成员或部门；
    /// <c>limit</c> 默认 50、最多 200，且<b>分别</b>控制在职与离职数据的数量（实际返回条数可能更少）。
    /// </para>
    /// <para>分页：<c>is_last = false</c> 时须带上次返回的 <c>next_cursor</c> 继续调用。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SearchServiceContactsRequest"/>：auth_corpid 必填 /
    /// query_word 必填且最长 64 个 UTF-8 字符 / query_type / query_range / agentid / limit /
    /// full_match_field / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>查询结果（is_last / query_result{user、party、dismiss_user} / next_cursor）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91844"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/contact/search")]
    Task<SearchServiceContactsResponse> SearchContactsAsync(
        [Body] SearchServiceContactsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量通讯录搜索
    /// <para>一次提交多个搜索关键词（官方限制：每次搜索列表数量<b>不超过 50</b>），逐项返回各自的结果与游标。</para>
    /// <para>
    /// <b>官方逐请求错误形态（关键陷阱）</b>：某条请求失败时错误落在结果项的 <c>errcode</c> / <c>errmsg</c>
    /// 且该项 <c>errcode</c> 非 0，<b>外层响应 errcode 仍为 0</b> ⇒ 调用方必须逐项判定，
    /// 不得只看外层返回码（否则失败项会被当作「无结果」静默吞掉）。
    /// </para>
    /// <para><c>auth_corpid</c> 与 <c>agentid</c> 提到批量请求<b>外层</b>，列表项内不重复携带。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchSearchServiceContactsRequest"/>：auth_corpid 必填 /
    /// agentid / query_request_list 必填且 ≤50 条）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>结果列表（query_result_list，元素含 query_request / query_result / next_cursor / is_last / errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91844"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/contact/batchsearch")]
    Task<BatchSearchServiceContactsResponse> BatchSearchContactsAsync(
        [Body] BatchSearchServiceContactsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通讯录 id 转译（异步）
    /// <para>把文档里的成员 / 部门 id 转译为企业可见的形式，输入为已上传的临时素材
    /// （<c>media_id_list</c>，官方限制<b>不超过 20 个文件</b>，只支持 xls/xlsx、doc/docx、csv、txt，
    /// 其中 csv/txt <b>必须为 UTF-8 编码</b>）。</para>
    /// <para>
    /// <b>官方条件必填规则</b>：<c>output_file_name</c> 在文件个数 &gt; 1 时<b>必填</b>；
    /// 个数 = 1 且未填时转译完<b>不</b>打包成 zip。<c>output_file_format</c> 当前<b>仅支持</b>输出 pdf，
    /// 不指定则与输入同格式。
    /// </para>
    /// <para>
    /// <b>异步契约</b>：本端点只回 <c>jobid</c>（最大 64 字节），结果须另调
    /// <see cref="IWechatWorkServiceBatchService.GetServiceBatchJobResultAsync"/>（官方 91882）轮询。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="TranslateServiceContactIdsRequest"/>：auth_corpid /
    /// media_id_list 必填 / output_file_name / output_file_format）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务 id（jobid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91846"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/contact/id_translate")]
    Task<TranslateServiceContactIdsResponse> TranslateContactIdsAsync(
        [Body] TranslateServiceContactIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通讯录成员排序
    /// <para>对给定的 userid 列表按「姓名拼音 / 别名」排序，常用于选人组件的服务端预排序。</para>
    /// <para>
    /// <b>官方约束</b>：<c>useridlist</c> 最多支持 <b>1000</b> 个；<c>sort_options</c> 指定多个时按
    /// <b>优先级</b>排序（先按第一项，相同再按第二项）；<c>sort_field</c> 0 姓名拼音 / 1 别名，
    /// <c>sort_type</c> 0 升序 / 1 降序。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SortServiceContactsRequest"/>：auth_corpid /
    /// useridlist 必填且 ≤1000 / sort_options）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>排序后的 userid 列表（字段名与请求同为 <c>useridlist</c>）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92093"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/contact/sort")]
    Task<SortServiceContactsResponse> SortContactsAsync(
        [Body] SortServiceContactsRequest request,
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「微信『一物一码』」域 SDK（6 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/service/api/"/>
/// → 微信「一物一码」。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>业务闭环（5 步，SDK 只提供端点、不编排）</b>：
/// <see cref="ApplyCodeAsync"/>（按数量批量申请，得申请单号）
/// → <see cref="QueryCodeApplicationAsync"/>（轮询至 <c>status = FINISH</c>，取码段）
/// → <see cref="DownloadCodePackageAsync"/>（按码段下载二维码包）
/// → <see cref="ActivateCodeAsync"/>（按码段激活并绑定小程序跳转信息）
/// → <see cref="QueryCodeActivationAsync"/>（按申请单 + 偏移量 / 原始码查询激活信息）；
/// 另有 <see cref="ExchangeCodeTicketAsync"/>（用户扫码所得临时票据换取正式营销码）。
/// </para>
/// <para>
/// <b>立项前置检查单结论（方案 §4.4 四项，逐页核验产出，勿弱化）</b>：
/// </para>
/// <list type="number">
/// <item><b>账号门槛</b>：6 页适用范围<b>均为「服务号（需申请）」</b>——注意措辞是「<b>需申请</b>」
/// （非模板消息 / OCR 域的「仅认证」）⇒ 须先向官方申请开通该能力，未开通即不可调用
/// （SDK 不做本地闸，由官方错误码表达）。本域<b>无</b>主体配额类错误码。</item>
/// <item><b>票据体系</b>：<b>无新票据体系</b>——6 端点全部消费应用级 <see cref="MpTokenTypes.AccessToken"/>
/// （Query 注入）。下载所得的 <c>buffer</c> 需 <b>base64 decode + 解密</b>（官方未给出算法与密钥来源，
/// 见 <see cref="MpCodeDownloadResponse"/> remarks），与令牌体系无关。</item>
/// <item><b>支付耦合</b>：<b>不耦合</b>。</item>
/// <item><b>第三方平台支持面</b>：6 页<b>全部</b>支持代商家调用，权限集 id 统一为 <b>46</b> ——
/// 按 M0-R3 裁决只在 XML 记录，不扩实现面（与 F5 联动）。</item>
/// </list>
/// <para>
/// <b>域级形态与约束（逐页核验）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>响应为平级字段</b>（根即 <c>errcode</c>/<c>errmsg</c> + 业务字段，<b>无 <c>data</c> 包裹</b>）
/// ——与门店域 <c>data</c> 包裹形态不同，照官方各页形态建模。</item>
/// <item><b>频率限制</b>：6 页官方<b>均无频率章节、无频次数值</b>（SDK 不编造）。</item>
/// <item><b>错误码面极薄</b>：6 页错误码表<b>仅列通用码 <c>40001</c></b>（<c>applycode</c> 页另误挂了与本接口
/// 无关的 <c>40002 invalid grant_type</c>）⇒ 本域<b>不新增任何错误码常量</b>，失败面由通用码表达。</item>
/// <item><b>幂等键</b>：官方原文「相同 <c>isv_application_id</c> 视为同一申请单」⇒ 申请具备幂等语义。</item>
/// <item><b>数量下限</b>：官方原文「<c>code_count</c> 必须是 <b>10000 的整数倍</b>，范围 <b>10000-20000000</b>」
/// ——无法小额申请；SDK 不本地拦截。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。<c>tickettocode</c> 页官方未列 Query 参数表
/// （疑漏，见 <see cref="ExchangeCodeTicketAsync"/> remarks）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OneCode", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpOneCodeService
{
    /// <summary>
    /// 申请二维码（批量生成指定数量的营销码）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/onecode/api_intp_marketcode_applycode.html"/>
    /// （官方接口英文名 <c>intp_marketcode_applycode</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">申请请求（<c>code_count</c> 10000 的整数倍且 ∈ [10000, 20000000]；<c>isv_application_id</c> 为幂等键）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请单号（<c>application_id</c>）；<b>不含二维码本体</b>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpApplyCodeRequest"/>；
    /// 官方注意事项原文「<c>code_count</c> 参数必须是 10000 的整数倍，范围 10000-20000000」。
    /// </para>
    /// <para>官方错误码：<c>0</c>（官方把成功码也列入表，照录）/ <c>40001</c> / <c>40002</c>（误挂，见类型级 remarks）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/intp/marketcode/applycode")]
    Task<MpApplyCodeResponse> ApplyCodeAsync(
        [Body] MpApplyCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询二维码申请单。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/onecode/api_intp_marketcode_applycodequery.html"/>
    /// （官方接口英文名 <c>intp_marketcode_applycodequery</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>application_id</c> 或 <c>isv_application_id</c> 二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请单状态（<c>status</c>，<c>FINISH</c> 方可下载）+ 码段列表（<c>code_generate_list</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpApplyCodeQueryRequest"/>；
    /// 官方注意事项原文「需要通过 <c>application_id</c> 或 <c>isv_application_id</c> 查询申请单状态」。
    /// </para>
    /// <para>
    /// <b>轮询语义（SDK 不编排）</b>：申请单为异步生成，须由宿主轮询本接口至
    /// <c>status = <see cref="MpCodeApplyStatuses.Finished"/></c>（官方未给状态枚举表与建议间隔）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/intp/marketcode/applycodequery")]
    Task<MpCodeApplyQueryResponse> QueryCodeApplicationAsync(
        [Body] MpApplyCodeQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载二维码包。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/onecode/api_intp_marketcode_applycodedownload.html"/>
    /// （官方接口英文名 <c>intp_marketcode_applycodedownload</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">下载请求（<c>application_id</c> + 码段 <c>code_start</c>/<c>code_end</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码包（<c>buffer</c>，base64；须宿主 decode 后解密）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpCodeDownloadRequest"/>;
    /// 官方注意事项原文「<b>下载前需确保申请单状态为 FINISH</b>」（前置校验归宿主编排，SDK 不拦）。
    /// </para>
    /// <para>
    /// <b>响应形态（官方文档缺陷，SDK 建模裁决）</b>：官方类型列写「formdata 文件 buffer」（非合法 JSON 类型，
    /// 无法据以建模），而同页返回示例为 JSON 包裹的 <c>{"errcode","errmsg","buffer"}</c> ⇒ SDK 按<b>示例</b>
    /// 建模为字符串（走既有 JSON 管线，<b>不新建下载通道</b>）；官方「解密参见 3.1」指向<b>不存在</b>的章节。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/intp/marketcode/applycodedownload")]
    Task<MpCodeDownloadResponse> DownloadCodePackageAsync(
        [Body] MpCodeDownloadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 激活二维码（指定范围）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/onecode/api_intp_marketcode_codeactive.html"/>
    /// （官方接口英文名 <c>intp_marketcode_codeactive</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">激活请求（分析维度字段须规范命名 + 扫码跳转小程序信息 + 码段闭区间）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpCodeActiveRequest"/>；
    /// 官方原文「<c>code_start</c>/<c>code_end</c> 为闭区间（包含该值）」。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）；官方注意事项原文「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/intp/marketcode/codeactive")]
    Task<MpResponse> ActivateCodeAsync(
        [Body] MpCodeActiveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询二维码激活状态。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/onecode/api_intp_marketcode_codeactivequery.html"/>
    /// （官方接口英文名 <c>intp_marketcode_codeactivequery</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>application_id</c>+<c>code_index</c> 或 <c>code</c>/<c>code_url</c> 两路径互斥）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>原始码与对应激活信息（<see cref="MpCodeInfoResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpCodeActiveQueryRequest"/>；
    /// 官方注意事项原文「支持通过 <c>application_id</c>+<c>code_index</c> 或 <c>code</c>/URL 两种查询方式」。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/intp/marketcode/codeactivequery")]
    Task<MpCodeInfoResponse> QueryCodeActivationAsync(
        [Body] MpCodeActiveQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// CODE_TICKET 换 CODE（将用户扫码所得的临时票据转换为正式营销码）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/onecode/api_intp_marketcode_tickettocode.html"/>
    /// （官方接口英文名 <c>intp_marketcode_tickettocode</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">兑换请求（<c>openid</c> + <c>code_ticket</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>原始码与对应激活信息（与 <see cref="QueryCodeActivationAsync"/> 共用 <see cref="MpCodeInfoResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpTicketToCodeRequest"/>。
    /// <b>官方文档缺陷（照录）</b>：本页<b>未列 Query 参数表</b>（其余 5 页均列 <c>access_token</c>）⇒
    /// SDK 按同域契约仍以 Query 注入 <c>access_token</c>（第三方代调用为 <c>authorizer_access_token</c>）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）；官方注意事项原文「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/intp/marketcode/tickettocode")]
    Task<MpCodeInfoResponse> ExchangeCodeTicketAsync(
        [Body] MpTicketToCodeRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 小程序版本（官方 <c>wxa_type</c> 取值；激活二维码时用于绑定扫码跳转的小程序版本）。
/// </summary>
/// <remarks>官方原文：<c>wxa_type</c>「小程序版本，默认为 <c>0</c> 正式版，开发版为 <c>1</c>，体验版为 <c>2</c>」。</remarks>
public static class MpCodeWxaTypes
{
    /// <summary>正式版（官方 0，默认）。</summary>
    public const int Release = 0;

    /// <summary>开发版（官方 1）。</summary>
    public const int Develop = 1;

    /// <summary>体验版（官方 2）。</summary>
    public const int Trial = 2;
}

/// <summary>
/// 二维码申请单状态（官方 <c>applycodequery</c> 的 <c>status</c> 取值）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档缺陷（照录）</b>：官方<b>未给出状态枚举表</b>（仅说明为「申请单状态」），
/// 唯一出现的取值 <c>FINISH</c> 来自返回示例，且 <c>applycodedownload</c> 页的注意事项原文
/// 「下载前需确保申请单状态为 FINISH」佐证其为「生成完成」态。
/// </para>
/// <para>
/// SDK 只登记该<b>示例值</b>并标注枚举未定义 —— <b>不得据此推断出完整状态机</b>（如 <c>PROCESSING</c> 之类）；
/// 宿主轮询时应以「<b>非 <see cref="Finished"/></b>」而非「等于某中间态」为继续条件。
/// </para>
/// </remarks>
public static class MpCodeApplyStatuses
{
    /// <summary>生成完成（官方返回示例值；官方未给枚举表，SDK 只登记该值）。</summary>
    public const string Finished = "FINISH";
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「微信发票」域 SDK（17 端点：商户开票 5 + 开票平台 5 + 发票报销 4 + 极速开发票 3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/service/api/"/>
/// → 微信发票（商户开票 / 开票平台 / 发票报销 / 极速开发票）。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>立项前置检查单结论（方案 §4.4 四项，逐页核验产出，勿弱化）</b>：
/// </para>
/// <list type="number">
/// <item><b>账号门槛（两档并存，页间不一致，照录）</b>：商户开票族与插入 / 报销族为「公众号 ✔ / 服务号 ✔」；
/// <b>开票平台族（<c>platform/*</c>）为「公众号 / 服务号 —— <c>需申请</c>」</b>（须提交场景申请并审核通过后方可调用）；
/// <b>极速开发票的 <c>scantitle</c> 为「公众号 / 服务号 —— <c>仅认证</c>」</b>（三档措辞并存，SDK 不做本地闸）。</item>
/// <item><b>票据体系（本域的关键裁决）</b>：<b>17 页全部使用应用级 <see cref="MpTokenTypes.AccessToken"/></b>
/// （第三方为 <c>authorizer_access_token</c>），<b>无任何页面使用卡券 <c>api_ticket</c></b>
/// ⇒ <b>本域不引入 <c>api_ticket</c> 建模、不新增票据机制</b>（检查单「发票域例外」的担忧不成立）。
/// 两点澄清：① <c>getauthurl</c> 请求体的 <c>ticket</c> 是<b>授权页 ticket</b>（业务参数，非鉴权凭证；
/// 官方未说明来源，SDK 不代取 —— 同族「获取 sdk 临时票据」端点可作候选来源但官方未做关联说明）；
/// ② <c>s_pappid</c> / <c>spappid</c> 是<b>开票平台标识字符串</b>（由开票平台告知商户），非票据。</item>
/// <item><b>支付耦合</b>：<b>不耦合</b>——本域只消费微信支付<b>商户号</b>（<c>mchid</c>）作为配置标识，
/// 不调用任何支付接口、不处理支付通知。</item>
/// <item><b>第三方平台支持面</b>：17 页中 <b>16 页支持</b>代商家调用（权限集 <b>26</b>；
/// <c>seturl</c> 与 <c>insert</c> 为 <b>8、26</b>）；<b><c>scantitle</c> 是唯一「不支持第三方平台调用」的端点</b>
/// —— 按 M0-R3 裁决只在 XML 记录，不扩实现面（与 F5 联动）。</item>
/// </list>
/// <para>
/// <b>域与拆分裁决</b>：官方同属「微信发票」<b>一个分组</b>，4 个子族共享同一权限面与同一条业务状态链
/// （<c>createcard</c> → <c>insert</c> → 报销状态更新），故<b>单域 17 端点承载</b>
/// （对齐数据统计域「同官方分组 21 端点单域」先例；拆 4 域只会让守卫与上下文翻倍而语义无收益）。
/// </para>
/// <para>
/// <b>业务闭环（SDK 只提供端点、不编排）</b>：
/// 开票平台侧：<see cref="GetInvoicePlatformIdentifyAsync"/>（取 <c>invoice_url</c> 并解析 <c>s_pappid</c>）
/// → <see cref="SetInvoiceBizAttrAsync"/>（<c>set_auth_field</c>/<c>set_pay_mch</c>/<c>set_contact</c> 一次性配置）
/// → <see cref="CreateInvoiceCardAsync"/>（建模板得 <c>card_id</c>）；
/// 商户侧：<see cref="GetInvoiceAuthUrlAsync"/>（取授权页链接，用户授权后绑定 order_id）
/// → <see cref="UploadInvoicePdfAsync"/>（得 <c>s_media_id</c>，<b>3 天有效</b>）
/// → <see cref="InsertInvoiceAsync"/>（插卡）；
/// 报销侧：<see cref="GetReimburseInvoiceAsync"/> / <see cref="BatchGetReimburseInvoicesAsync"/>
/// → <see cref="UpdateReimburseInvoiceStatusAsync"/> / <see cref="BatchUpdateReimburseInvoiceStatusAsync"/>。
/// </para>
/// <para>
/// <b>频率限制</b>：17 页官方<b>均无频率章节、无频次数值</b>（SDK 不编造数值）。
/// </para>
/// <para>
/// <b>错误码面（族级共用）</b>：官方在多个页面<b>重复列出同一张族级错误码表</b>
/// （约 43 枚，含 <c>73000</c>~<c>73110</c> 开票平台协议错误），且明显与具体端点不匹配
/// （如 <c>createcard</c> 页列出插卡 / 票据领取场景码）⇒ SDK 按<b>族级共用错误码面</b>建模
/// （去重后 <c>40078</c> 与 <c>72015</c>~<c>72063</c>、<c>73000</c>~<c>73110</c>），不逐端点重复。
/// </para>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Invoice", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpInvoiceService
{
    /// <summary>
    /// 查询与设置授权页与商户信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/auth/api_invoicebizsetattr.html"/>
    /// （官方接口英文名 <c>invoicebizsetattr</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="action">功能类型（官方 Query <c>action</c>，必填；六值见 <c>MpInvoiceBizAttrActions</c>）。</param>
    /// <param name="request">请求体（按 <paramref name="action"/> 三选一携带对应字段；三个 <c>get_*</c> 传空体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>被设置 / 被查询的字段（<c>auth_field</c>/<c>paymch_info</c>/<c>contact</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，<b>Query</b> 携带 <c>action</c>（必填）与 <c>access_token</c>；
    /// 请求体见 <see cref="MpInvoiceBizAttrRequest"/>（与响应同构，共用三个字段类型）。
    /// </para>
    /// <para>
    /// <b>前置顺序（官方正文原文，SDK 不编排）</b>：<c>set_contact</c> 需在<b>获取授权链接之前</b>先设置；
    /// <c>set_auth_field</c>/<c>set_pay_mch</c> 为<b>一次性设置</b>（除非调整字段 / 识别号变更或更换开票平台）；
    /// 使用 <c>type=0</c> 或 <c>type=2</c> 授权页时<b>无需调用</b> <c>set_auth_field</c>。
    /// </para>
    /// <para>官方错误码：<c>72028</c>（未设置支付商户信息）/ <c>72029</c>（未设置授权字段）/ <c>72063</c>（联系方式为空）/ 族级错误码面其余项。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/setbizattr")]
    Task<MpInvoiceBizAttrResponse> SetInvoiceBizAttrAsync(
        [Query("action")] string action,
        [Body] MpInvoiceBizAttrRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询授权信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/FiscalReceipt/api_invoicebizgetauthdata.html"/>
    /// （官方接口英文名 <c>invoiceBizGetAuthData</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> + <c>s_pappid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票状态（<c>invoice_status</c>）与授权时间戳（<c>auth_time</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceAuthDataRequest"/>；本页官方适用范围为「公众号 ✔ / 服务号 ✔」。</para>
    /// <para>官方错误码：<c>40078</c> / <c>72015</c> / <c>72031</c> / <c>72035</c> / <c>72036</c> / <c>72038</c> / <c>72040</c> / <c>72042</c> / <c>72043</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/getauthdata")]
    Task<MpInvoiceAuthDataResponse> QueryInvoiceAuthDataAsync(
        [Body] MpInvoiceAuthDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取授权页链接。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/auth/api_invoicebizgetauthurl.html"/>
    /// （官方接口英文名 <c>invoicebizgetauthurl</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">请求（订单号 / 金额（分）/ 来源 / 授权页 ticket / 授权类型；见 <see cref="MpInvoiceAuthUrlRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权链接（<c>auth_url</c>）；<c>appid</c> 仅 <c>source = wxa</c> 时返回。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceAuthUrlRequest"/>；
    /// <b>授权类型 <c>type</c> 三形态</b>（<c>0</c> 申请开票 / <c>1</c> 填写抬头申请开票 / <c>2</c> 领取发票），
    /// 官方原文「<b>使用支付后开票业务时，只能调用 type=1 类型</b>」。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/getauthurl")]
    Task<MpInvoiceAuthUrlResponse> GetInvoiceAuthUrlAsync(
        [Body] MpInvoiceAuthUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝开票。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/FiscalReceipt/api_invoicebizrejectinsert.html"/>
    /// （官方接口英文名 <c>invoicebizrejectinsert</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">拒绝请求（<c>s_pappid</c> + <c>order_id</c> + <c>reason</c> 必填；<c>url</c> 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceRejectInsertRequest"/>。
    /// <b>不可逆语义（官方原文，勿弱化）</b>：拒绝后该订单<b>无法向用户再次开票</b>；
    /// 需重新开票必须使用<b>新的 <c>order_id</c></b> 并重新获取授权链接。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/rejectinsert")]
    Task<MpResponse> RejectInsertAsync(
        [Body] MpInvoiceRejectInsertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取开票平台识别码。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/FiscalReceipt/api_setinvoiceurl.html"/>
    /// （官方接口英文名 <c>setinvoiceurl</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该开票平台专用授权链接（<c>invoice_url</c>）；<c>s_pappid</c> 须<b>从该链接内解析</b>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，<b>无请求体</b>，Query 仅携带 <c>access_token</c>。</para>
    /// <para>
    /// 官方文档缺陷（照录）：接口名为「获取开票平台识别码」但返回<b>无独立识别码字段</b>
    /// （<c>s_pappid</c> 需从 <c>invoice_url</c> 解析）；本页把 <c>errcode</c> 标为 <c>string</c>（全站罕见）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/seturl")]
    Task<MpInvoiceSetUrlResponse> GetInvoicePlatformIdentifyAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传发票 PDF。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/FiscalReceipt/api_invoiceplatformsetpdf.html"/>
    /// （官方接口英文名 <c>invoiceplatformsetpdf</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>pdf</c></b>；
    /// 由调用方构建 <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>PDF 素材 id（<c>s_media_id</c>，<b>有效期 3 天</b>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>pdf</c>。
    /// </para>
    /// <para>
    /// <b>3 天有效期（官方原文，勿弱化）</b>：PDF 若 3 天内未关联到发票卡券并发送到用户卡包<b>将被清理</b>
    /// （3 天后仍要关联须<b>重新上传</b>）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/platform/setpdf")]
    Task<MpInvoiceSetPdfResponse> UploadInvoicePdfAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询已上传的 PDF 文件。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/FiscalReceipt/api_invoiceplatformgetpdf.html"/>
    /// （官方接口英文名 <c>invoiceplatformgetpdf</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>action</c> 填 <c>get_url</c> + <c>s_media_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>PDF 的 URL（<c>pdf_url</c>，<b>两小时有效</b>）与过期时间（<c>pdf_url_expire_time</c>，7200 秒）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceGetPdfRequest"/>。</para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/platform/getpdf")]
    Task<MpInvoiceGetPdfResponse> GetInvoicePdfAsync(
        [Body] MpInvoiceGetPdfRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新发票卡券状态。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/FiscalReceipt/api_invoicekpupdatainvoicestatus.html"/>
    /// （官方接口英文名 <c>invoicekpupdatainvoicestatus</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">更新请求（<c>card_id</c>/<c>code</c>/<c>reimburse_status</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceUpdateStatusRequest"/>；
    /// 用于发票平台在发票状态变化（如被冲红、被报销）时更新用户卡包中发票卡券的状态。
    /// </para>
    /// <para>
    /// <b>冲红语义（官方原文）</b>：电子发票冲红时将 <c>reimburse_status</c> 置为
    /// <c>INVOICE_REIMBURSE_CANCEL</c>，「表现为对应的发票卡券被核销」（与 <c>CANCEL</c> 的语义表述冲突，照录）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/platform/updatestatus")]
    Task<MpResponse> UpdateInvoiceCardStatusAsync(
        [Body] MpInvoiceUpdateStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建发票卡券模板。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/platform/api_invoiceplatformcreatecard.html"/>
    /// （官方接口英文名 <c>invoiceplatformcreatecard</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">模板请求（<c>invoice_info</c>；<c>title</c> ≤9 汉字、<c>logo_url</c> 须走永久素材接口）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票卡券模板编号（<c>card_id</c>；后续插卡必填）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceCreateCardRequest"/>；
    /// <b>适用范围为「公众号 / 服务号 —— 需申请」</b>（须提交场景申请并审核通过）。
    /// </para>
    /// <para>
    /// 官方文档缺陷（照录）：本页错误码表与「创建卡券模板」场景不匹配（大量为插卡 / 票据领取场景码，
    /// 属族级表整段复用）；请求示例含尾随逗号且示例值两端带空格。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/platform/createcard")]
    Task<MpInvoiceCreateCardResponse> CreateInvoiceCardAsync(
        [Body] MpInvoiceCreateCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将电子发票卡券插入用户卡包。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/platform/api_insertinvoice.html"/>
    /// （官方接口英文名 <c>insertinvoice</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">插卡请求（<c>order_id</c>/<c>card_id</c>/<c>appid</c>/<c>card_ext</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票 <c>code</c> / 用户 <c>openid</c> / <c>unionid</c>（绑定开放平台后才有）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceInsertRequest"/>；
    /// 本接口由开票平台或自建平台商户调用，须在用户已完成授权（<see cref="GetInvoiceAuthUrlAsync"/>）后调用。
    /// </para>
    /// <para>
    /// <b>调用方口径矛盾（照录）</b>：官方接口描述要求「必须使用之前调用获取 <c>s_pappid</c> 接口时的
    /// <b>开票平台公众号 appid</b> 调用，否则插卡失败」，而字段表把 <c>appid</c> 说明为「一般为<b>商户 appid</b>」。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/insert")]
    Task<MpInvoiceInsertResponse> InsertInvoiceAsync(
        [Body] MpInvoiceInsertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询报销发票信息（单张）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/reimbursement/api_invoicebxgetinvoice.html"/>
    /// （官方接口英文名 <c>invoicebxgetinvoice</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>card_id</c> + <c>encrypt_code</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票结构化信息 + 用户发票信息（<c>user_info</c>，与插卡请求同构 ⇒ 共用 DTO）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceReimburseQueryRequest"/>。
    /// <b>官方文档缺陷（照录）</b>：官方请求体栏标「无」而代码示例给出两个字段 ⇒ SDK 按示例建模。
    /// </para>
    /// <para>
    /// 官方发票状态码（原文）：<c>INVOICE_REIMBURSE_INIT</c> 初始 / <c>INVOICE_REIMBURSE_LOCK</c> 已锁定 /
    /// <c>INVOICE_REIMBURSE_CLOSURE</c> 已核销（见 <c>MpInvoiceReimburseStatuses</c>）。
    /// </para>
    /// <para>官方错误码：<c>0</c>（官方把成功码也列入表，照录）/ <c>72015</c> / <c>72017</c> / <c>72023</c> / <c>72024</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/reimburse/getinvoiceinfo")]
    Task<MpInvoiceReimburseInfo> GetReimburseInvoiceAsync(
        [Body] MpInvoiceReimburseQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新报销发票状态（单张锁定 / 解锁 / 报销）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/reimbursement/api_invoicebxupdatainvoicestatus.html"/>
    /// （官方接口英文名 <c>invoicebxupdatainvoicestatus</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">更新请求（<c>card_id</c> + <c>encrypt_code</c> + <c>reimburse_status</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceReimburseUpdateRequest"/>；
    /// <b>报销状态不可逆</b>（报销后发票从用户卡包移除）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/reimburse/updateinvoicestatus")]
    Task<MpResponse> UpdateReimburseInvoiceStatusAsync(
        [Body] MpInvoiceReimburseUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新报销发票状态。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/reimbursement/api_invoicereimburseupdatestatusbatch.html"/>
    /// （官方接口英文名 <c>invoicereimburseupdatestatusbatch</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">批量更新请求（<c>openid</c> + <c>reimburse_status</c> + <c>invoice_list</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceReimburseBatchUpdateRequest"/>。
    /// <b>事务性语义（官方注意事项原文，勿弱化）</b>：其中一张失败则<b>整批回滚</b>
    /// （其它发票状态恢复到调用前）；报销方须在报销 / 锁定 / 解锁后<b>及时同步状态至微信侧</b>；
    /// <b>报销状态不可逆</b>。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/reimburse/updatestatusbatch")]
    Task<MpResponse> BatchUpdateReimburseInvoiceStatusAsync(
        [Body] MpInvoiceReimburseBatchUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取报销发票信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/reimbursement/api_invoicereimbursegetinvoicebatch.html"/>
    /// （官方接口英文名 <c>invoicereimbursegetinvoicebatch</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">批量查询请求（<c>item_list</c>，条目为 <c>card_id</c> + <c>encrypt_code</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票列表（<c>item_list</c>；条目与单张查询响应共用 <see cref="DataModels.Invoice.MpInvoiceReimburseInfo"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceReimburseBatchGetRequest"/>；
    /// 官方文档缺陷（照录）：<c>item_list</c> <b>未标注最大条数</b>（SDK 不本地拦截）。
    /// </para>
    /// <para>官方错误码：<c>0</c>（官方描述「ok 或者 in a normal state」，照录）/ <c>72015</c> / <c>72017</c> / <c>72023</c> / <c>72024</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/reimburse/getinvoicebatch")]
    Task<MpInvoiceReimburseBatchGetResponse> BatchGetReimburseInvoicesAsync(
        [Body] MpInvoiceReimburseBatchGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 录入抬头到用户微信。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/name/api_invoicebizgetusertitleurl.html"/>
    /// （官方接口英文名 <c>invoicebizgetusertitleurl</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">抬头信息（<c>user_fill</c> 决定企业设置或用户填写；税号须 15-20 位数字或字母）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户确认链接（<c>url</c>；需将链接发送给用户确认）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceUserTitleUrlRequest"/>；
    /// 官方适用范围为「公众号 ✔ / 服务号 ✔ / 移动应用 ✔」。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/biz/getusertitleurl")]
    Task<MpInvoiceUserTitleUrlResponse> GetUserTitleUrlAsync(
        [Body] MpInvoiceUserTitleUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商户专属抬头链接。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/name/api_invoicebizgetselecttitleurl.html"/>
    /// （官方接口英文名 <c>invoicebizgetselecttitleurl</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">请求（<c>attach</c> 附加字段 + <c>biz_name</c> 商户名称）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>专属抬头链接（<c>url</c>；官方原文「<b>需将链接转为二维码展示</b>」）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceSelectTitleUrlRequest"/>。</para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/biz/getselecttitleurl")]
    Task<MpInvoiceSelectTitleUrlResponse> GetSelectTitleUrlAsync(
        [Body] MpInvoiceSelectTitleUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解析扫描的抬头二维码。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/invoice/name/api_invoicescantitle.html"/>
    /// （官方接口英文名 <c>invoicescantitle</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">解析请求（<c>scan_text</c> 扫码原始数据）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>抬头结构化信息（<c>title_type</c> 0 单位 / 1 个人 + title/phone/tax_no/addr/bank_type/bank_no）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpInvoiceScanTitleRequest"/>。
    /// <b>本端点是本域唯一「不支持第三方平台调用」的端点</b>（其余 16 页均支持，权限集 26 / 8、26）；
    /// 适用范围为「公众号 / 服务号 —— <b>仅认证</b>」（与开票平台族的「需申请」不同档）。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/scantitle")]
    Task<MpInvoiceScanTitleResponse> ScanInvoiceTitleAsync(
        [Body] MpInvoiceScanTitleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开具电子发票。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Invoice/E_Invoice/Vendor_API_List.html#14"/>
    /// （官方接口英文名 <c>makeOutInvoice</c>）。
    /// </summary>
    /// <param name="request">开票请求（票面信息见 <see cref="MpInvoiceBillingBody"/>；官方契约顶层仅 <c>invoiceinfo</c> 一键）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（开票结果查 <see cref="QueryInvoiceInfoAsync"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"invoiceinfo": {…}}</c>；
    /// <c>fpqqlsh</c>（发票请求流水号）同一商户号下唯一；开票为<b>异步</b>——本端点受理成功不代表开票完成。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> 等（本页错误码表照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/makeoutinvoice")]
    Task<MpResponse> MakeOutInvoiceAsync(
        [Body] MpMakeOutInvoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 冲红电子发票。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Invoice/E_Invoice/Vendor_API_List.html#15"/>
    /// （官方接口英文名 <c>clearOutInvoice</c>）。
    /// </summary>
    /// <param name="request">冲红请求（<c>invoiceinfo</c> 内须填写原发票代码 <c>yfpdm</c> / 原发票号码 <c>yfphm</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"invoiceinfo": {…}}</c>。
    /// <b>覆盖删除语义</b>：冲红后原发票作废、生成红字发票，不可逆。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/clearoutinvoice")]
    Task<MpResponse> ClearOutInvoiceAsync(
        [Body] MpClearOutInvoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询电子发票开具结果。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Invoice/E_Invoice/Vendor_API_List.html#16"/>
    /// （官方接口英文名 <c>queryInvoiceInfo</c>；<b>官方路由拼写为 queryinvoceinfo（ce），照抄不修正</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>fpqqlsh</c> + <c>nsrsbh</c>，均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>票面详情（发票代码 / 号码 / 开票日期 / 校验码 / PDF 地址；开票处理中字段可能缺省）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{fpqqlsh, nsrsbh}</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/invoice/queryinvoceinfo")]
    Task<MpQueryInvoiceInfoResponse> QueryInvoiceInfoAsync(
        [Body] MpQueryInvoiceInfoRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 授权页与商户信息操作类型（官方 <c>setbizattr</c> 的 Query <c>action</c> 取值）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档缺陷（照录）：官方<b>请求示例的 action 与用例不符</b>——「设置授权页字段信息」的示例 URL 用
/// <c>action=set_pay_mch</c> 而请求体为 <c>auth_field</c>（应为 <c>set_auth_field</c>）；
/// 「查询授权页字段信息」用 <c>action=get_pay_mch</c> 而返回为 <c>auth_field</c>（应为 <c>get_auth_field</c>）。
/// </para>
/// <para>三个 <c>get_*</c> 的请求体为空（传 <c>{}</c>）。</para>
/// </remarks>
public static class MpInvoiceBizAttrActions
{
    /// <summary>设置授权页字段（请求体携带 <c>auth_field</c>）。</summary>
    public const string SetAuthField = "set_auth_field";

    /// <summary>查询授权页字段（请求体为空）。</summary>
    public const string GetAuthField = "get_auth_field";

    /// <summary>设置微信支付商户号与开票平台关系（请求体携带 <c>paymch_info</c>）。</summary>
    public const string SetPayMch = "set_pay_mch";

    /// <summary>查询微信支付商户号与开票平台关系（请求体为空）。</summary>
    public const string GetPayMch = "get_pay_mch";

    /// <summary>设置联系方式（请求体携带 <c>contact</c>；须在获取授权链接之前）。</summary>
    public const string SetContact = "set_contact";

    /// <summary>查询联系方式（请求体为空）。</summary>
    public const string GetContact = "get_contact";
}

/// <summary>
/// 开票来源（官方 <c>getauthurl</c> 的 <c>source</c> 取值）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：<c>redirect_url</c> 说明称「只有在 <c>source</c> 为 <b>H5</b> 时需要填写」，
/// 但合法枚举中<b>不存在 H5</b>（疑指 <see cref="Web"/>）。
/// </remarks>
public static class MpInvoiceAuthSources
{
    /// <summary>app 开票（从外部拉起微信，授权完自动回原 app，无需填 <c>redirect_url</c>）。</summary>
    public const string App = "app";

    /// <summary>微信 H5 开票（官方 <c>redirect_url</c> 说明中疑指本值）。</summary>
    public const string Web = "web";

    /// <summary>小程序开发票。</summary>
    public const string Wxa = "wxa";

    /// <summary>普通网页开票。</summary>
    public const string Wap = "wap";
}

/// <summary>
/// 授权页类型（官方 <c>getauthurl</c> 的 <c>type</c> 取值）。
/// </summary>
/// <remarks>
/// 官方原文：<b>「使用支付后开票业务时，只能调用 type=1 类型」</b>。
/// 官方文档缺陷（照录）：正文与参数表对三值的措辞不一致（正文「申请开票 / 填写抬头申请开票 / 领取发票」
/// vs 参数表「开票授权 / 填写字段开票授权 / 领票授权」）。
/// </remarks>
public static class MpInvoiceAuthUrlTypes
{
    /// <summary>开票授权（申请开票；商户已从其它渠道获得用户抬头时使用）。</summary>
    public const int Open = 0;

    /// <summary>填写字段开票授权（填写抬头申请开票；<b>支付后开票业务只能使用本类型</b>）。</summary>
    public const int FillFields = 1;

    /// <summary>领票授权（领取发票；发票已开具成功后归集到卡包）。</summary>
    public const int Collect = 2;
}

/// <summary>
/// 发票报销状态（官方 <c>reimburse_status</c> 取值；三值来自 <c>getinvoiceinfo</c>/<c>updatestatusbatch</c> 页的状态说明，
/// 第四值来自 <c>platform/updatestatus</c> 页的冲红说明）。
/// </summary>
/// <remarks>
/// <para>
/// 官方原文（状态含义）：「<c>INVOICE_REIMBURSE_INIT</c> 发票初始状态，未锁定，可提交报销；
/// <c>INVOICE_REIMBURSE_LOCK</c> 发票已锁定，无法重复提交报销；
/// <c>INVOICE_REIMBURSE_CLOSURE</c> 发票已核销，从用户卡包中移除」。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>platform/updatestatus</c> 页仅列出两值（<c>INIT</c> 与冲红用的 <c>CANCEL</c>）
/// 且称「具体发票状态见发票状态一览表」——<b>该表不在该页</b>；<c>reimburse/updateinvoicestatus</c> 页
/// 亦仅给出示例值 <c>INIT</c> 而无完整枚举表。
/// </para>
/// </remarks>
public static class MpInvoiceReimburseStatuses
{
    /// <summary>发票初始状态（未锁定，可提交报销）。</summary>
    public const string Init = "INVOICE_REIMBURSE_INIT";

    /// <summary>发票已锁定（无法重复提交报销）。</summary>
    public const string Lock = "INVOICE_REIMBURSE_LOCK";

    /// <summary>发票已核销（从用户卡包中移除）。</summary>
    public const string Closure = "INVOICE_REIMBURSE_CLOSURE";

    /// <summary>发票已取消（<c>platform/updatestatus</c> 页用于<b>电子发票冲红</b>，官方描述为「表现为卡券被核销」）。</summary>
    public const string Cancel = "INVOICE_REIMBURSE_CANCEL";
}

/// <summary>
/// 抬头类型（官方 <c>scantitle</c> 的 <c>title_type</c> 取值）。
/// </summary>
public static class MpInvoiceTitleTypes
{
    /// <summary>单位抬头（官方 0）。</summary>
    public const int Business = 0;

    /// <summary>个人抬头（官方 1）。</summary>
    public const int Personal = 1;
}

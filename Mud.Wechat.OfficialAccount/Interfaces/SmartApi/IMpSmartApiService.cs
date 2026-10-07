// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「智能接口」域 SDK（12 端点：AI 开放接口 3 + OCR 识别 7 + 图像处理 2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/service/api/"/>
/// → 智能接口（AI 开放接口 / OCR 识别 / 图像处理）。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>三种路径前缀照实建模于同一域（前缀不入命名空间）</b>：AI 开放接口在
/// <c>/cgi-bin/media/voice/*</c>、OCR 在 <c>/cv/ocr/*</c>、图像处理在 <c>/cv/img/*</c>
/// ——官方同属「智能接口」分组且请求形态同质（multipart 上传 + Query 传参 + 同一错误码面），
/// 沿用订阅通知域「双前缀单域」的既存先例，不按前缀拆域。
/// </para>
/// <para>
/// <b>双调用形态 = 双方法（官方契约，勿合并）</b>：OCR 7 + 图像处理 2 共 9 个端点的官方请求为
/// <b>二选一互斥形态</b>——form-data 实时上传 <c>img</c>，<b>或</b> Query 传 <c>img_url</c>
/// （官方原文「传这个则不用传另一个」，由微信后台下载图片识别）。SDK 为每个端点建
/// <c>*ByUploadAsync</c>（multipart）与 <c>*ByUrlAsync</c>（Query）两个方法：
/// 单方法「可选 <c>IFormContent?</c>」形态被否决——生成管线对 <c>[MultipartForm]</c> 参数无条件生成
/// <c>HttpRequest.Content = formData</c>，传 <see langword="null"/> 将运行时 NRE。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>账号适用范围</b>：AI 开放接口三端点官方表为「小程序 / 公众号 / 服务号 / 小游戏 / 移动应用 —— 全部 ✔」；
/// OCR 与图像处理九端点为「小程序 ✔ / 公众号 <b>仅认证</b> / 服务号 <b>仅认证</b>」
/// （SDK 不做账号类型本地闸，由官方错误码表达）。</item>
/// <item><b>频率上限（不编造数值）</b>：OCR 七端点官方原文「次数限制为 <b>100 次/天</b>」，
/// 其中<b>菜单识别页无频率章节且注明「尚未接入服务平台，暂时不支持付费购买」</b>；
/// 图像处理两端点与 AI 开放接口三端点官方全页<b>无频率数值</b>。</item>
/// <item><b>图片限制</b>：官方原文「文件大小限制：<b>小于 2M</b>」；OCR 页错误码 <c>101002</c> 另引
/// 「<c>resp_type = 0: 2MB</c>，<c>resp_type = 1: 10MB</c>」但全文未定义 <c>resp_type</c>
/// ——两处矛盾照录，SDK 不做本地大小拦截（由官方错误码表达）。</item>
/// <item><b>第三方平台</b>：OCR 与图像处理九端点官方明示支持代商家调用（<b>权限集 id 117</b>）；
/// AI 开放接口三端点官方明示<b>不支持</b>云调用与第三方平台调用 —— 按 M0-R3 裁决只在 XML 记录，不扩实现面。</item>
/// <item><b>不做业务编排</b>：AI 语音「上传 → 10s 内轮询」的两步语义只写进 XML，SDK 不封装轮询策略。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。<c>img_url</c> / <c>format</c> /
/// <c>voice_id</c> / <c>lang</c> / <c>lfrom</c> / <c>lto</c> / <c>ratios</c> 均为<b>非凭据</b> Query 参数。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "SmartApi", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpSmartApiService
{
    // ------------------------------------------------------------------ AI 开放接口（3）

    /// <summary>
    /// 上传语音文件（进行转文字识别）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_addvoicetorecofortext.html"/>
    /// （官方接口英文名 <c>addVoiceTorecoForText</c>）。
    /// </summary>
    /// <param name="format">语音文件格式（<b>必填</b>；官方原文「只支持 mp3、16k、单声道、最大 1M」，见 <see cref="MpVoiceFileFormats"/>）。</param>
    /// <param name="voiceId">语音唯一标识（<b>必填</b>；后续 <c>queryrecoresultfortext</c> 以同一 id 查询）。</param>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>media</c></b>；
    /// 由调用方构建 <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>）。</param>
    /// <param name="lang">语言（官方 <c>lang</c>，可选，<c>zh_CN</c> 或 <c>en_US</c>，默认中文；见 <see cref="MpVoiceLanguages"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST multipart/form-data</b>，<b>Query</b> 携带 <c>format</c>（必填）/ <c>voice_id</c>（必填）/
    /// <c>lang</c>（可选）/ <c>access_token</c>；表单文件字段名为 <c>media</c>。
    /// 官方「注意事项」原文为「本接口无特殊注意事项」（但字段说明含 16k/单声道/最大 1M 硬限制，矛盾照录）。
    /// </para>
    /// <para>
    /// <b>两步语义（官方注意事项，SDK 不编排）</b>：本接口上传后须在 <b>10 秒内</b>调用
    /// <see cref="QueryVoiceRecognitionResultAsync"/> 取回识别结果。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40010</c>（invalid voice size，不合法的语音文件大小）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/media/voice/addvoicetorecofortext")]
    Task<MpResponse> UploadVoiceForRecognitionAsync(
        [Query("format")] string format,
        [Query("voice_id")] string voiceId,
        [MultipartForm] IFormContent formData,
        [Query("lang")] string? lang = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取语音识别结果。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_queryrecoresultfortext.html"/>
    /// （官方接口英文名 <c>queryRecoResultForText</c>）。
    /// </summary>
    /// <param name="voiceId">语音唯一标识（<b>必填</b>，与上传时一致）。</param>
    /// <param name="lang">语言（官方 <c>lang</c>，可选，<c>zh_CN</c> 或 <c>en_US</c>，默认中文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>识别结果（<c>result</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，<b>无请求体</b>，Query 携带 <c>voice_id</c>（必填）/ <c>lang</c>（可选）/ <c>access_token</c>。
    /// </para>
    /// <para>
    /// 官方「注意事项」原文：<b>「添加完文件之后 10s 内调用这个接口」</b>
    /// ⇒ 轮询与退避策略归宿主（「SDK 不做业务编排」红线）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c>（本页错误码表仅此两行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/media/voice/queryrecoresultfortext")]
    Task<MpVoiceRecoResultResponse> QueryVoiceRecognitionResultAsync(
        [Query("voice_id")] string voiceId,
        [Query("lang")] string? lang = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信翻译（文本内容翻译）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_translatecontent.html"/>
    /// （官方接口英文名 <c>translateContent</c>）。
    /// </summary>
    /// <param name="fromLanguage">源语言（<b>必填</b>，官方枚举仅 <c>zh_CN</c>/<c>en_US</c>）。</param>
    /// <param name="toLanguage">目标语言（<b>必填</b>，官方枚举仅 <c>zh_CN</c>/<c>en_US</c>）。</param>
    /// <param name="request">翻译请求体（<c>content</c> 必填，utf8，最大 600Byte）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>原文与译文（<c>from_content</c>/<c>to_content</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，<b>Query</b> 携带 <c>lfrom</c>/<c>lto</c>（均必填）与 <c>access_token</c>；
    /// 请求体 <c>{"content": …}</c>（<b>最大 600Byte</b>，SDK 不本地拦截）。
    /// </para>
    /// <para>
    /// <b>路径语义异常（官方现状，照录）</b>：本接口为文本翻译，但路径位于 <c>/cgi-bin/media/voice/</c> 下。
    /// 官方「注意事项」原文「本接口无特殊注意事项」（与 600Byte / 语言枚举两处硬限制矛盾，照录）。
    /// </para>
    /// <para>官方错误码：<c>40001</c> / <c>40035</c>（invalid args size，不合法的参数）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/media/voice/translatecontent")]
    Task<MpVoiceTranslateResponse> TranslateContentAsync(
        [Query("lfrom")] string fromLanguage,
        [Query("lto")] string toLanguage,
        [Body] MpVoiceTranslateRequest request,
        CancellationToken cancellationToken = default);

    // ------------------------------------------------------------------ OCR 识别（7 × 双形态）

    /// <summary>
    /// 身份证识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_idcardocr.html"/>
    /// （官方接口英文名 <c>idcardOcr</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>身份证正反面识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>官方原文：「图片支持使用 <c>img</c> 参数实时上传，也支持使用 <c>img_url</c> 参数传送图片地址，由微信后台下载图片进行识别」
    /// —— 两形态互斥，URL 形态见 <see cref="IdCardOcrByUrlAsync"/>。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101002</c> / <c>101003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/idcard")]
    Task<MpOcrIdCardResponse> IdCardOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 身份证识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_idcardocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>；由微信后台下载识别）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>身份证正反面识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>与 <see cref="IdCardOcrByUploadAsync"/> 互斥（官方原文「传这个则不用传 img 参数」）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/idcard")]
    Task<MpOcrIdCardResponse> IdCardOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 银行卡识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bankcardocr.html"/>
    /// （官方接口英文名 <c>bankcardOcr</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>银行卡卡面识别结果（<c>number</c> / <c>id</c> 双键照录，见 <see cref="DataModels.SmartApi.MpOcrBankCardResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/bankcard")]
    Task<MpOcrBankCardResponse> BankCardOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 银行卡识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bankcardocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>银行卡卡面识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/bankcard")]
    Task<MpOcrBankCardResponse> BankCardOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 行驶证识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivingocr.html"/>
    /// （官方接口英文名 <c>drivingocr</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>行驶证识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/driving")]
    Task<MpOcrDrivingResponse> DrivingOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 行驶证识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivingocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>行驶证识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/driving")]
    Task<MpOcrDrivingResponse> DrivingOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 驾驶证识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivinglicenseocr.html"/>
    /// （官方接口英文名 <c>drivinglicenseocr</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>驾驶证识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/drivinglicense")]
    Task<MpOcrDrivingLicenseResponse> DrivingLicenseOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 驾驶证识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivinglicenseocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>驾驶证识别结果。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/drivinglicense")]
    Task<MpOcrDrivingLicenseResponse> DrivingLicenseOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 营业执照识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bizlicenseocr.html"/>
    /// （官方接口英文名 <c>bizlicenseOcr</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>营业执照识别结果（仅返回图片中存在的字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>官方「注意事项」原文：「返回字段仅包含当前营业执照图片中存在的字段，若对应字段不存在则不返回」。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101002</c> / <c>101003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/bizlicense")]
    Task<MpOcrBizLicenseResponse> BizLicenseOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 营业执照识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bizlicenseocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>营业执照识别结果（仅返回图片中存在的字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/bizlicense")]
    Task<MpOcrBizLicenseResponse> BizLicenseOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通用印刷体识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_commocr.html"/>
    /// （官方接口英文名 <c>commocr</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>印刷体识别条目列表（<c>items[]</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>
    /// （本页官方把 <c>img</c> 标为非必填，但两形态实为至少择一，照录）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101002</c> / <c>101003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/comm")]
    Task<MpOcrCommResponse> CommOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通用印刷体识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_commocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>印刷体识别条目列表（<c>items[]</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/comm")]
    Task<MpOcrCommResponse> CommOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 菜单识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_menuocr.html"/>
    /// （官方接口英文名 <c>menuOcr</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，官方原文「文件大小限制：小于 2M」）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>菜单内容（<c>content.menu_items[]</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>
    /// <b>本页无频率上限</b>（官方全页无频次章节，且原文「该接口尚未接入服务平台，暂时不支持付费购买」）
    /// ——不得套用其它 OCR 页的 100 次/天。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>101000</c> / <c>101001</c> / <c>101002</c>（本页错误码表未列 <c>40001</c>，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/menu")]
    Task<MpOcrMenuResponse> MenuOcrByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 菜单识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_menuocr.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>菜单内容（<c>content.menu_items[]</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/ocr/menu")]
    Task<MpOcrMenuResponse> MenuOcrByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);

    // ------------------------------------------------------------------ 图像处理（2 × 双形态）

    /// <summary>
    /// 图片智能裁剪（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgaicrop.html"/>
    /// （官方接口英文名 <c>imgAiCrop</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，大小 &lt; 2M）。</param>
    /// <param name="ratios">宽高比（官方 form 字段 <c>ratios</c>，可选；多个以英文逗号「,」分隔，<b>最多 5 个</b>；
    /// 为空则算法自动裁剪最佳宽高比）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>智能裁剪结果（<c>results[]</c>：裁剪区域四边界 + <c>img_size</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；
    /// 表单字段 <c>img</c>（文件）与 <c>ratios</c>（可选）。</para>
    /// <para>官方「注意事项」原文：「如果提供多个宽高比，请以英文逗号「,」分隔，最多支持 5 个宽高比」。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c>（invalid image url）/ <c>101002</c>（invalid image data）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/img/aicrop")]
    Task<MpImageAiCropResponse> AiCropByUploadAsync(
        [MultipartForm] IFormContent formData,
        [Form(FieldName = "ratios")] string? ratios = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 图片智能裁剪（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgaicrop.html"/>。
    /// </summary>
    /// <param name="imgUrl">要检测的图片 URL（官方 Query <c>img_url</c>；由微信后台下载识别）。</param>
    /// <param name="ratios">宽高比（官方 form 字段 <c>ratios</c>，可选；多个以英文逗号「,」分隔，最多 5 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>智能裁剪结果（<c>results[]</c> + <c>img_size</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，Query 携带 <c>img_url</c> 与 <c>access_token</c>；<c>ratios</c> 为表单字段。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/img/aicrop")]
    Task<MpImageAiCropResponse> AiCropByUrlAsync(
        [Query("img_url")] string imgUrl,
        [Form(FieldName = "ratios")] string? ratios = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 二维码/条码识别（图片实时上传形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgqrcode.html"/>
    /// （官方接口英文名 <c>imgQrcode</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>img</c></b>，官方原文「文件需小于 2MB」）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>码识别结果（<c>code_results[]</c>：类型 / 信息 / 坐标 + <c>img_size</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST multipart/form-data</b>，Query 携带 <c>access_token</c>；表单文件字段名 <c>img</c>。</para>
    /// <para>
    /// 官方原文：「支持条码、二维码、DataMatrix 和 PDF417 的识别」；「<b>二维码、DataMatrix 会返回位置坐标，
    /// 条码和 PDF417 暂不返回位置坐标</b>」⇒ 条目坐标可空。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101002</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/img/qrcode")]
    Task<MpImageQrcodeResponse> QrcodeRecognitionByUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 二维码/条码识别（图片 URL 形态）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgqrcode.html"/>。
    /// </summary>
    /// <param name="imgUrl">图片 URL（官方 Query <c>img_url</c>；由微信后台下载识别）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>码识别结果（<c>code_results[]</c> + <c>img_size</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>（无请求体），Query 携带 <c>img_url</c> 与 <c>access_token</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cv/img/qrcode")]
    Task<MpImageQrcodeResponse> QrcodeRecognitionByUrlAsync(
        [Query("img_url")] string imgUrl,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 语音文件格式（官方 <c>format</c> Query 参数取值）。
/// </summary>
/// <remarks>
/// 官方原文（AI 开放接口·上传语音文件）：「<c>format</c>：文件格式，<b>只支持 mp3、16k、单声道、最大 1M</b>」
/// ——官方仅给出 <c>mp3</c> 一个枚举值，其余为编码约束（非取值），故本表只有一项。
/// </remarks>
public static class MpVoiceFileFormats
{
    /// <summary>mp3（16k 采样率、单声道、最大 1M——官方编码约束原文）。</summary>
    public const string Mp3 = "mp3";
}

/// <summary>
/// 语音 / 翻译语言（官方 <c>lang</c> / <c>lfrom</c> / <c>lto</c> 参数取值）。
/// </summary>
/// <remarks>
/// 官方枚举仅两值：<c>zh_CN</c>（默认中文）/ <c>en_US</c>。语音识别与微信翻译共用同一枚举表。
/// </remarks>
public static class MpVoiceLanguages
{
    /// <summary>简体中文（官方 <c>zh_CN</c>，默认值）。</summary>
    public const string ZhCn = "zh_CN";

    /// <summary>英文（官方 <c>en_US</c>）。</summary>
    public const string EnUs = "en_US";
}

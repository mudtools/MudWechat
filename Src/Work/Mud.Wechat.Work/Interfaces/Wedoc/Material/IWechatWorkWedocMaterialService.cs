// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「文档」模块素材管理域公共 SDK
/// （上传图片 + 上传文件 + 文件分块上传（初始化/分块/完成），3 个官方文档页对应 5 个端点）。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocMaterialService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocMaterialService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocMaterialService"/>。
/// </para>
/// <para>官方契约形态：「文件分块上传」官方单文档页承载 3 条路由
/// （<c>file_upload_init</c> / <c>file_upload_part</c> / <c>file_upload_finish</c>），
/// 与整文件上传的 <c>file_upload</c> 并存——同域两形态，勿混用。</para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 高级功能账号管理族见 <see cref="IWechatWorkWedocVipService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)</c> 建立作用域后再调用。
/// </para>
/// <para>权限口径（官方三页同口径的权限表原文）：自建应用「需配置到“可调用应用”列表中的应用 secret 所获取的 accesstoken 来调用」；
/// 第三方应用与代开发自建应用「需具有“文档”权限」。<b>分块上传</b>页另在权限表首行多出一句
/// 「企业需要使用“微盘”secret所获取的accesstoken来调用」（官方原文如此，与同页另三行并存）。</para>
/// <para>官方频率口径：素材管理三页（99933 / 101870 / 101871）<b>均无</b>独立的数值型频率限制，
/// 走官方「附录 - 访问频率限制」的默认阈值（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时，
/// <see href="https://developer.work.weixin.qq.com/document/path/90312"/>）；
/// 本域各端点<b>不</b>写「接口调用频率限制为 N 次/分钟」——该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。
/// 官方给出的<b>规模</b>限制是逐端点的（整文件 10M、分块文件 20G、分块大小固定 2M），已写入各端点文档。</para>
/// <para>
/// 素材内容以 Base64 字符串形态承载于请求体字段（<c>base64_content</c> / <c>file_base64_content</c>）：
/// SDK 侧按字符串透传、不做编解码，也不做本地大小硬拦，越界由应答 <c>errcode</c> 表达；
/// 官方明确要求<b>只填 Base64 本体</b>，不得携带 <c>data:application/x-javascript;base64</c> 之类的数据类型描述前缀。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocMaterialService
{
    /// <summary>
    /// 上传图片
    /// <para>向指定文档上传图片素材（图片内容以 Base64 承载）。</para>
    /// <para>官方字段形态：图片内容字段名为 <c>base64_content</c>（区别于上传文件的 <c>file_base64_content</c>）；
    /// 响应回传图片链接与宽高、大小（官方字段名 <c>url</c>，非 <c>image_url</c>）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UploadWedocImageRequest"/>：docid / base64_content，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片链接与元信息（url / width / height / size）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99933"/>（官方标题「上传文档图片」）</para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101875"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101879"/></para>
    /// <para>官方权限：自建应用需配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；第三方应用与代开发自建应用需具有「文档」权限。</para>
    /// <para>官方业务限制：本接口为 POST；<c>docid</c> 官方口径「文档 ID，通过新建文档接口创建后获得」，
    /// <c>base64_content</c> 为 base64 之后的图片内容；本页面未给出图片大小上限与频率数字（走官方默认阈值，见本接口汇总口径）。</para>
    /// <para>官方响应口径：<c>url</c> 为图片 url；<c>height</c> / <c>width</c> / <c>size</c> 三字段官方<b>参数表标为 string、
    /// 返回示例却是裸数字</b>（如 <code>"size": 81259</code>），本仓按示例口径以数值类型（<c>int?</c> / <c>long?</c>）承载
    /// ——示例即线格式的直接证据，参数表的类型列在本页与示例自相矛盾。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/image_upload")]
    Task<UploadWedocImageResponse> UploadImageAsync(
        [Body] UploadWedocImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传文件
    /// <para>向指定文档上传文件素材（整文件一次性上传，文件内容以 Base64 承载）。</para>
    /// <para>官方限制：内容超出整文件上传承载能力时须改用「文件分块上传」三端点
    /// （<see cref="InitFileUploadAsync"/> → <see cref="UploadFilePartAsync"/> → <see cref="FinishFileUploadAsync"/>）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UploadWedocFileRequest"/>：docid / file_name / file_base64_content，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件链接（file_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101870"/>（官方标题「上传文档文件」）</para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101876"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101880"/></para>
    /// <para>官方权限：自建应用需配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；第三方应用与代开发自建应用需具有「文档」权限。</para>
    /// <para>官方限制（本页正文数字）：<c>file_name</c> <b>最多填 255 个字符，英文算 1 个、汉字算 2 个</b>；
    /// <c>file_base64_content</c> 的<b>文件大小上限为 10M</b>，大于 10M 的文件官方要求改用「文件分块上传」三端点；
    /// 且官方明确「只需要填入文件内容的 Base64，不需要添加任何如 <c>data:application/x-javascript;base64</c> 的数据类型描述信息」。
    /// 本页未给出独立的数值型频率限制，走官方默认阈值（见本接口汇总口径）。</para>
    /// <para>官方业务限制：本接口为 POST；<c>docid</c> 官方口径「文档 id，通过新建文档接口创建后获得」；响应只有 <c>file_url</c>（官方称「新建文件的 url」）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/file_upload")]
    Task<UploadWedocFileResponse> UploadFileAsync(
        [Body] UploadWedocFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 文件分块上传初始化
    /// <para>按文件分块的 sha 值列表登记本次分块上传，返回分块上传凭证或直接命中秒传。</para>
    /// <para>官方字段形态：<c>size</c> 与 <c>block_sha</c>（按分块顺序排列的数组）官方必填；
    /// 响应以 <c>hit_exist</c> 判别后续流程——命中时无须再调用分块与完成端点，未命中时以 <c>upload_key</c> 续传。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="InitWedocFileUploadRequest"/>：docid / file_name / size / block_sha，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否命中秒传（hit_exist）、文件上传凭证（upload_key，官方可选返回）与文件链接（file_url，官方可选返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101871"/>（官方标题「分块上传文档文件」）</para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101877"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101881"/></para>
    /// <para>与分块上传、分块上传完成共用同一官方文档页（一页三路由）。</para>
    /// <para>官方权限：本页权限表首行为「企业需要使用“微盘”secret所获取的accesstoken来调用」，其后三行为
    /// 自建应用配置到「可调用应用」列表、第三方与代开发需具有「文档」权限（官方原文四行并存，本域按 access_token 链路调用）。</para>
    /// <para>官方限制（本页正文数字）：<b>分块大小固定为 2M（2097152 字节）</b>；<c>size</c> 官方标注<b>最大支持 20G</b>；
    /// <c>block_sha</c> 为「文件分块<b>累积</b> sha 值，按分块顺序填入数组」，官方附录给出的算法是：按 2M 分块后对每一块依次
    /// <c>sha1_update</c>，每次 update 后<b>记录当前 state 的十六进制</b>作为该块的累积 sha 值，最后一块（可能小于 2M）
    /// 在 update 完之后再 <c>sha1_final</c>，即整文件的 sha1 作为最后一块的值。</para>
    /// <para>官方业务限制：本接口为 POST，<c>docid</c> / <c>file_name</c> / <c>size</c> / <c>block_sha</c> 官方均必填；
    /// 响应以 <c>hit_exist</c>（官方为 JSON 布尔）判别后续流程——命中秒传时官方直接回 <c>file_url</c> 且<b>上传流程即完成</b>，
    /// 不命中时才回 <c>upload_key</c> 供 <see cref="UploadFilePartAsync"/> 与 <see cref="FinishFileUploadAsync"/> 使用。</para>
    /// <para>本页未给出独立的数值型频率限制，走官方默认阈值（见本接口汇总口径）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/file_upload_init")]
    Task<InitWedocFileUploadResponse> InitFileUploadAsync(
        [Body] InitWedocFileUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传文件分块
    /// <para>按 <see cref="InitFileUploadAsync"/> 返回的凭证上传单个文件分块（分块内容以 Base64 承载）。</para>
    /// <para>官方字段形态：分块内容字段沿用整文件上传的 <c>file_base64_content</c>；本端点仅返回 errcode / errmsg。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UploadWedocFilePartRequest"/>：upload_key / index / file_base64_content，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101871"/>（官方标题「分块上传文档文件」）</para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101877"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101881"/></para>
    /// <para>与分块上传初始化、分块上传完成共用同一官方文档页（一页三路由）。</para>
    /// <para>官方限制：本接口为 POST，<c>upload_key</c> / <c>index</c> / <c>file_base64_content</c> 官方均必填；
    /// <c>index</c> 官方口径为「文件分块号。文件内容按 2M 分块，<b>从 1 开始</b>」（不是 0 基）；
    /// 分块内容字段沿用整文件上传的 <c>file_base64_content</c> 且同样「只要 Base64 本体、不带数据类型描述前缀」；
    /// 官方对分块上传的建议是「<b>可并发上传，并发数建议不超过 10</b>」。</para>
    /// <para>本页未给出独立的数值型频率限制，走官方默认阈值（见本接口汇总口径）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/file_upload_part")]
    Task<WechatWorkResponse> UploadFilePartAsync(
        [Body] UploadWedocFilePartRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 文件分块上传完成
    /// <para>全部分块上传结束后以凭证提交完成，返回文件链接。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="FinishWedocFileUploadRequest"/>：upload_key，官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件链接（file_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101871"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101877"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101881"/></para>
    /// <para>与分块上传初始化、分块上传共用同一官方文档页（一页三路由）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/file_upload_finish")]
    Task<FinishWedocFileUploadResponse> FinishFileUploadAsync(
        [Body] FinishWedocFileUploadRequest request,
        CancellationToken cancellationToken = default);
}

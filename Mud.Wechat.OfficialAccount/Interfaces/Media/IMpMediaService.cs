// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「素材管理」域 SDK（JSON 通道：临时 / 永久素材上传、计数、列表、删除、
/// uploadimg；下载端点见 <c>IMpMediaDownloadService</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 素材管理（临时素材 / 永久素材）。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>双通道分工（I3 裁决，勿合并）</b>：素材域是官方 API 面里首个「响应为二进制流」的域 ——
/// 上传端点（响应 JSON）落本接口走生成管线；下载端点（<c>media/get</c> / <c>media/get/jssdk</c>，
/// 响应为文件流）落 <c>IMpMediaDownloadService</c> 走独立 HttpClient 请求形态
/// （<see cref="Mud.HttpUtils.IBaseHttpClient.SendRawAsync"/> 原始响应直读 + Content-Type 分支判错，
/// 不进 JSON 反序列化管线）。两通道同属 <see cref="MpModule.Media"/> 注册组。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>临时素材 3 天有效</b>：官方「注意事项」原文「媒体文件在微信后台保存时间为 3 天，
/// 3 天后 media_id 失效」；<c>media_id</c> 可复用；用于多媒体消息的 media_id 最多可复用 <b>10 条</b>。</item>
/// <item><b>类型与大小限制（与公众平台官网一致）</b>：图片（image）10M，PNG/JPEG/JPG/GIF；
/// 语音（voice）2M、播放长度不超过 60s，AMR/MP3；视频（video）10M，MP4；缩略图（thumb）64KB，JPG。
/// 官方页面「注意事项」另写「图片 2MB / 视频 10MB」——两处表述<b>不一致</b>，SDK 以字段表
/// （10M）为口径并把矛盾记录在 remarks，不据其做本地拦截（由官方错误码表达）。</item>
/// <item><b>服务器端调用</b>：官方原文「接口须在服务器端调用，不可在前端（小程序、网页、APP 等）直接调用」。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：上传端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。<c>type</c> 亦为 Query 参数（官方契约）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Media", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpMediaService
{
    /// <summary>
    /// 新增临时素材。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/temporary/api_uploadtempmedia.html"/>
    /// （官方接口英文名 <c>mediaUpload</c>）。
    /// </summary>
    /// <param name="type">媒体文件类型：<c>image</c>/<c>voice</c>/<c>video</c>/<c>thumb</c>（<see cref="MpMediaTypes"/>）。</param>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>media</c></b>；
    /// 由调用方构建 <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>；
    /// 大小与格式限制见类型级 remarks——SDK 不做本地拦截）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上传结果（<c>type</c>/<c>media_id</c>/<c>created_at</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST multipart/form-data</b>，<b>Query</b> 携带 <c>type</c>（必填）与
    /// <c>access_token</c>；表单文件字段名为 <c>media</c>（含 filename / filelength / content-type）。
    /// </para>
    /// <para>
    /// <b>临时素材 3 天有效且可复用</b>（官方「注意事项」原文）；用于多媒体消息的 media_id
    /// 最多可复用 10 条。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40004</c>（invalid media type，不合法的媒体文件类型）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/media/upload")]
    Task<MpUploadTempMediaResponse> UploadTempMediaAsync(
        [Query("type")] string type,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传永久素材。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_addmaterial.html"/>
    /// （官方接口英文名 <c>addMaterial</c>）。
    /// </summary>
    /// <param name="type">媒体类型：image / voice / video / thumb（<see cref="MpMediaTypes"/>；图文不可直接上传）。</param>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>media</c></b>；
    /// 由调用方构建 <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>）。</param>
    /// <param name="description">素材描述 JSON 字符串（官方表单字段 <c>description</c>；<b>上传视频素材时需要</b>，
    /// 形态 <c>{"title":…,"introduction":…}</c>——可经 <see cref="MpMaterialDescription"/> +
    /// <c>MediaJsonContext</c> 序列化得到；官方字段表标可选但正文写「视频需要」，矛盾已照录，SDK 不做本地拦截）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上传结果（<c>media_id</c>；<c>url</c> 仅图片素材返回）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST multipart/form-data</b>，<b>Query</b> 携带 <c>type</c>（必填）与
    /// <c>access_token</c>；表单文件字段名 <c>media</c>；视频另附表单字段 <c>description</c>（JSON 字符串）。
    /// </para>
    /// <para>
    /// <b>大小限制（官方字段表原文，与临时素材页差异照录：永久图片支持 bmp）</b>：
    /// 图片（image）10M，bmp/png/jpeg/jpg/gif；语音（voice）2M、≤60s，mp3/wma/wav/amr；
    /// 视频（video）10M，MP4；缩略图（thumb）64KB，JPG。
    /// </para>
    /// <para>
    /// <b>数量上限（官方「注意事项」原文）</b>：「公众号的素材库保存总数量有上限：图文消息素材、
    /// 图片素材上限为 <b>100000</b>，其他类型为 <b>1000</b>」。
    /// </para>
    /// <para>
    /// <b>图片 URL 域名限制</b>：官方原文「永久图片素材新增后，将带有 URL 返回给开发者，
    /// 开发者可以在腾讯系域名内使用（腾讯系域名外使用，图片将被屏蔽）」。
    /// </para>
    /// <para>官方错误码：<c>40007</c>（invalid media_id，本页错误码表仅此一行，照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/material/add_material")]
    Task<MpAddMaterialResponse> AddMaterialAsync(
        [Query("type")] string type,
        [MultipartForm] IFormContent formData,
        [Form(FieldName = "description")] string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取永久素材总数。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_getmaterialcount.html"/>
    /// （官方接口英文名 <c>getMaterialCount</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>四类素材计数（<c>voice_count</c>/<c>video_count</c>/<c>image_count</c>/<c>news_count</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，无请求体；路径为 <c>get_materialcount</c>（count 前无下划线，照抄官方）。
    /// 成功响应不含 <c>errcode</c>（缺省 0 视为成功）。
    /// </para>
    /// <para>
    /// <b>数量上限（官方「注意事项」原文）</b>：「图片和图文消息素材（包括单图文和多图文）的总数上限为
    /// <b>100000</b>，其他素材的总数上限为 <b>1000</b>」；「永久素材的总数包含公众平台官网素材管理中的素材」。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c>（本页 -1 的解决方案官方原文为「系统错误」，措辞与它页不一致，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/material/get_materialcount")]
    Task<MpGetMaterialCountResponse> GetMaterialCountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取永久素材列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_batchgetmaterial.html"/>
    /// （官方接口英文名 <c>batchGetMaterial</c>）。
    /// </summary>
    /// <param name="request">分页请求（<c>type</c> 必填；<c>offset</c> 从 0 起；<c>count</c> 取值 <b>1~20</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>素材列表（<c>total_count</c>/<c>item_count</c>/<c>item</c>；条目形态按 type 分流，见 <see cref="MpMaterialListItem"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"type":…,"offset":…,"count":…}</c>。
    /// </para>
    /// <para>
    /// <b>条目双形态（超集 DTO 承载）</b>：news 类型条目为 <c>{media_id, content.news_item[], update_time}</c>；
    /// image/voice/video 类型条目为 <c>{media_id, name, update_time, url}</c>（官方同字段表声明两形态）。
    /// </para>
    /// <para>
    /// 官方「注意事项」：①「包含公众平台官网新建的图文消息、语音、视频等素材」；
    /// ②「<b>临时素材无法通过本接口获取</b>」；③「需 https 协议调用」。
    /// </para>
    /// <para>
    /// 官方文档缺陷（照录）：列表页 <c>news_item.thumb_url</c> 的官方描述原文为
    /// 「图文消息的封面图片素材id」——是 id 的描述而非 URL 的描述，疑为官方复制粘贴错误。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40004</c> / <c>40007</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/material/batchget_material")]
    Task<MpBatchGetMaterialResponse> BatchGetMaterialAsync(
        [Body] MpBatchGetMaterialRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除永久素材。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_delmaterial.html"/>
    /// （官方接口英文名 <c>delMaterial</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>media_id</c> 必填；与 <c>get_material</c> 请求体字段集一致 ⇒ 共用 DTO）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方「注意事项」原文：①「<b>请谨慎操作本接口</b>，可以删除官网素材管理模块中的图文/语音/视频等素材
    /// （需先通过获取素材列表获取 media_id）」；②「<b>临时素材无法通过本接口删除</b>」；③「调用该接口需 https 协议」。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40007</c>（本页错误码表把成功码 0 也列入，照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/material/del_material")]
    Task<MpResponse> DelMaterialAsync(
        [Body] MpMediaIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传发表内容中的图片（官方现名；即图文消息内的图片 <c>uploadimg</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_uploadimage.html"/>
    /// （官方接口英文名 <c>uploadImage</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>media</c></b>；
    /// 由调用方构建 <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片 URL（<b>非 media_id</b>，无 media_id 生命周期）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST multipart/form-data</b>（表单字段 <c>media</c>；本端点<b>无 type 参数</b>）。
    /// </para>
    /// <para>
    /// <b>限制（官方原文）</b>：「图片仅支持 jpg/png 格式，大小必须在 <b>1MB</b> 以下」；
    /// 「不占用公众号的素材库中图片数量的 100000 个的限制」；本接口用于上传「发表内容」（文章或贴图）所需图片，
    /// 群发图文正文与草稿正文均使用本端点返回的 url。
    /// </para>
    /// <para>
    /// <b>归属说明（只建模一次）</b>：官方索引页把本端点同时列于「群发消息」与「永久素材」分组
    /// ⇒ 落素材域（本接口），群发域文档交叉引用本端点、不建第二份声明。
    /// </para>
    /// <para>官方错误码：<c>40005</c>（invalid file type）/ <c>40009</c>（invalid image size）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/media/uploadimg")]
    Task<MpUploadImageResponse> UploadImageAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 素材类型（官方 <c>type</c> Query 参数取值；临时素材四类 + 永久素材各页沿用同词表）。
/// </summary>
/// <remarks>
/// 值为官方字段原文（小写）：<c>image</c> 图片 / <c>voice</c> 语音 / <c>video</c> 视频 /
/// <c>thumb</c> 缩略图；永久素材另有 <c>news</c>（图文，仅查询类端点使用，不可上传）。
/// </remarks>
public static class MpMediaTypes
{
    /// <summary>图片（10M，PNG/JPEG/JPG/GIF）。</summary>
    public const string Image = "image";

    /// <summary>语音（2M、≤60s，AMR/MP3）。</summary>
    public const string Voice = "voice";

    /// <summary>视频（10M，MP4）。</summary>
    public const string Video = "video";

    /// <summary>缩略图（64KB，JPG）。</summary>
    public const string Thumb = "thumb";

    /// <summary>图文（<b>仅</b>永久素材查询/删除类端点的 type 取值，不可作为上传类型）。</summary>
    public const string News = "news";
}

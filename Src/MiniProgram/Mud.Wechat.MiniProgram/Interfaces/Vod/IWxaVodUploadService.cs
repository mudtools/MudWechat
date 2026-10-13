// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「短剧媒资 · 上传」域 SDK（6 端点：单个文件上传 / 拉取上传 / 查询任务 / 申请分片上传 /
/// 上传分片 / 确认上传；官方 minidrama 分区 → vod_fileupload）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页核验，minidrama 分区全集 31 端点）</b>：本接口取其中「上传」6 端点，
/// 其余按职责分到 <see cref="IWxaVodMediaService"/>（媒资管理 4）、<see cref="IWxaVodDramaService"/>（剧目提审与替换 7）、
/// <see cref="IWxaVodCdnService"/>（CDN 用量与流量包 3）、<see cref="IWxaVodAuthorizationService"/>（授权族 11）。
/// 五接口共用注册组 <c>Vod</c>。
/// </para>
/// <para>
/// <b>两条上传通路</b>：① <c>singlefileupload</c> 为<b>同步</b>单请求通路（应答直接给 <c>media_id</c>）；
/// ② <c>applyupload</c> → <c>uploadpart</c>（×N）→ <c>commitupload</c> 为分片通路，三步必须按序调用
/// （官方原文「调用该接口之前必须先调用申请分片上传接口」，<c>upload_id</c> 为串联键）。
/// <c>pullupload</c> 是第三条通路（官方按 URL 拉取），与 <c>gettask</c> 组成异步对。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 153），不新增令牌类型常量。第三方代调用形态（<c>authorizer_access_token</c>）
/// 由宿主以小程序应用配置供给令牌，本 SDK 不声明组件 / 授权方令牌（P3-R2）。
/// </para>
/// <para>
/// <b>适用范围字段异常（诚实留档，与直播域同一处置）</b>：31 页的「适用范围」段官方统一写
/// 「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用，请以实际调用情况为准」
/// —— <b>没有</b>「小程序」字样。本线归属判据取「令牌可用 <c>access_token</c>（小程序应用级）
/// + 文档树位于 <c>miniprogram/dev/server/API/minidrama</c>」两条，而非适用范围段。
/// </para>
/// <para>
/// <b>错误码表全域共用</b>：31 页给出<b>逐字相同</b>的一张 20 码表（<c>-2</c> / <c>-1</c> / <c>43002</c> /
/// <c>44002</c> / <c>47001</c> / <c>47003</c> / <c>10090001</c>~<c>10090043</c> / <c>10093011</c> /
/// <c>10093014</c> / <c>10093023</c> / <c>10093030</c>），守卫 DV5 锁定该「全域同表」事实 ⇒
/// 端点注释只列与本端点语义相关者，不复述全表。
/// </para>
/// <para>
/// <b>调用限制</b>：31 页官方均未公布额度数字（「本接口无特殊注意事项」或只写业务前置条件），
/// 与直播域的逐页额度不同；唯一数字型约束是 <c>part_number</c> 范围 1~100（即单次分片上传最多 100 片）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vod", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaVodUploadService
{
    /// <summary>
    /// 单个文件上传（<c>POST /wxa/sec/vod/singlefileupload</c>，<c>multipart/form-data</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_singlefileupload.html"/>。
    /// </summary>
    /// <param name="formData">
    /// multipart 表单内容（由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>，
    /// 生成器对该标注走 <see cref="IFormContent.ToHttpContentAsync"/> 通路）。官方表单字段名六支，须照原文：
    /// <c>media_name</c>（必填，命名格式「剧目名 - 对应剧集数」）/ <c>media_type</c>（必填，见 <see cref="WxaVodMediaTypes"/>）/
    /// <c>media_data</c>（必填，视频二进制）/ <c>cover_type</c>（见 <see cref="WxaVodCoverTypes"/>）/
    /// <c>cover_data</c>（封面二进制）/ <c>source_context</c>（上传完成事件透传）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒资 ID（<c>media_id</c>），见 <see cref="WxaVodMediaIdResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <c>multipart/form-data; boundary=&lt;delimiter&gt;</c>（官方原文注明箭头括号须替换为有效值）。</para>
    /// <para>
    /// <b>不声明请求体 DTO</b>：官方 <c>Content-Type</c> 是 multipart，JSON DTO 通路无法承载二进制字段
    /// （<c>media_data</c> / <c>cover_data</c> 官方类型 <c>buffer</c>）⇒ 表单由宿主拼装，守卫 DV2 断言本端点无请求 DTO。
    /// </para>
    /// <para><b>封面可省略</b>（官方原文）：不填 <c>cover_type</c>、<c>cover_data</c> 时默认截取视频首帧作为封面图。</para>
    /// <para><b>同步通路</b>：应答直接给 <c>media_id</c>，不产生任务、不需要 <c>gettask</c> 轮询（与 <see cref="PullUploadAsync"/> 相反）。</para>
    /// <para>官方错误码：<c>10090001</c>（视频类型不支持）/ <c>10090002</c>（图片类型不支持）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/singlefileupload")]
    Task<WxaVodMediaIdResponse> SingleFileUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取上传（<c>POST /wxa/sec/vod/pullupload</c>，异步，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_pullupload.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>media_name</c> + <c>media_url</c> 必填，<c>cover_url</c> / <c>source_context</c> 选填），见 <see cref="WxaVodPullUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务 ID（<c>task_id</c>），见 <see cref="WxaVodPullUploadResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；本接口为<b>异步</b>接口，上传完成会推送上传完成事件到开发者服务器，
    /// 也可以调用「查询任务」接口轮询上传结果（官方原文）。</para>
    /// <para><b>应答只有 <c>task_id</c>，没有 <c>media_id</c></b> ⇒ 拿到任务号不等于拿到媒资，媒资 ID 须从
    /// <see cref="GetTaskAsync"/> 的 <c>task_info.media_id</c> 取。</para>
    /// <para><b>不填 <c>cover_url</c> 时默认截取视频首帧作为封面图</b>（官方原文）。</para>
    /// <para><b>异步回读依赖消息推送配置</b>：上传完成事件走小程序消息服务器，未配置推送地址的宿主只能轮询 <see cref="GetTaskAsync"/>。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/pullupload")]
    Task<WxaVodPullUploadResponse> PullUploadAsync(
        [Body] WxaVodPullUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询任务（<c>POST /wxa/sec/vod/gettask</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_gettask.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>task_id</c> 一字段，必填），见 <see cref="WxaVodGetTaskRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务信息（<c>task_info</c>：状态 / 时间戳 / <c>media_id</c> / 任务级错误码），见 <see cref="WxaVodGetTaskResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>——「查询任务」这类只读端点官方同样使用 POST + JSON 请求体（本域 31 端点仅
    /// <c>authorizedrama</c> 一页写作 GET），误用 GET 触发共享表 <c>43002</c>「HTTP 请求必须使用 POST 方法」。</para>
    /// <para>
    /// <b>任务级错误码与接口级错误码分层</b>：外层 <c>errcode</c> 表示「查得到这个任务」，
    /// <c>task_info.errcode</c> 表示「这次上传的成败」（0 成功），状态口径见 <see cref="WxaVodTaskStatuses"/>、
    /// 任务类型见 <see cref="WxaVodTaskTypes"/>（官方当前只登记 <c>1 = 拉取上传任务</c>）。
    /// </para>
    /// </remarks>
    [Post("/wxa/sec/vod/gettask")]
    Task<WxaVodGetTaskResponse> GetTaskAsync(
        [Body] WxaVodGetTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请分片上传（<c>POST /wxa/sec/vod/applyupload</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_applyupload.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>media_name</c> + <c>media_type</c> 必填，<c>cover_type</c> / <c>source_context</c> 选填），见 <see cref="WxaVodApplyUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>本次分片上传的唯一标识（<c>upload_id</c>），见 <see cref="WxaVodApplyUploadResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；本端点是分片三步（apply → <see cref="UploadPartAsync"/> → <see cref="CommitUploadAsync"/>）的第一步。</para>
    /// <para>
    /// <b>分片上传没有「文件大小」入参</b>：官方请求体只有 <c>media_name</c> / <c>media_type</c> /
    /// <c>cover_type</c> / <c>source_context</c> 四字段（<c>singlefileupload</c> 才有 <c>media_data</c>），
    /// 分片归属由 <see cref="WxaVodResourceTypes"/> 区分。
    /// </para>
    /// <para><b><c>cover_type</c> 是「是否顺带上传封面」的开关</b>（官方原文）：填写即表明本次除媒体文件外还要上传封面图，不填则默认截取视频首帧。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/applyupload")]
    Task<WxaVodApplyUploadResponse> ApplyUploadAsync(
        [Body] WxaVodApplyUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传分片（<c>POST /wxa/sec/vod/uploadpart</c>，<c>multipart/form-data</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_uploadpart.html"/>。
    /// </summary>
    /// <param name="formData">
    /// multipart 表单内容（由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>）。官方表单字段四支：
    /// <c>upload_id</c>（必填，来自 <see cref="ApplyUploadAsync"/>）/ <c>part_number</c>（必填，范围 1~100）/
    /// <c>resource_type</c>（必填，见 <see cref="WxaVodResourceTypes"/>）/ <c>data</c>（必填，分片二进制，官方类型 <c>binary</c>）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该分片的 <c>etag</c>，见 <see cref="WxaVodUploadPartResponse"/>；须在 <see cref="CommitUploadAsync"/> 原样回填。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <c>multipart/form-data; boundary=&lt;delimiter&gt;</c>；调用前必须先调 <see cref="ApplyUploadAsync"/>（官方原文）。</para>
    /// <para>
    /// <b>两条 <c>part_number</c> 序列</b>（官方原文）：编号从 1 开始，「如除了上传视频外还需要上传封面图，
    /// 则封面图的 <c>part_number</c> 需重新从 1 开始编号」⇒ 视频与封面各自独立编号，靠 <c>resource_type</c> 区分归属，
    /// 混编触发 <c>10090005</c>（resource_type 无效）。
    /// </para>
    /// <para><b><c>etag</c> 值自带双引号</b>（官方示例）⇒ 原样保存并回填，不得自行加剥引号，见 <see cref="WxaVodPartInfo"/>。</para>
    /// <para><b>分片上限 100</b>（官方把 <c>part_number</c> 范围写成 1~100）⇒ 单文件体积上限等于「分片大小 × 100」，宿主侧切片须按此倒推。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/uploadpart")]
    Task<WxaVodUploadPartResponse> UploadPartAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认上传（<c>POST /wxa/sec/vod/commitupload</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_commitupload.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>upload_id</c> + <c>media_part_infos</c> 必填，<c>cover_part_infos</c> 选填），见 <see cref="WxaVodCommitUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒资 ID（<c>media_id</c>），见 <see cref="WxaVodMediaIdResponse"/>（与单个文件上传应答共用）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；调用前必须先调 <see cref="ApplyUploadAsync"/> 与 <see cref="UploadPartAsync"/>（官方原文）。</para>
    /// <para>
    /// <b>分片信息表必须有序</b>（官方原文）：<c>media_part_infos</c> 与 <c>cover_part_infos</c>
    /// 须按 <c>part_number</c> 从小到大排序、从 1 开始、连续且不重复。
    /// </para>
    /// <para>
    /// <b>官方参数表把 <c>media_part_infos</c> / <c>cover_part_infos</c> 列作 <c>object</c>、线格式却是数组</b>
    /// ⇒ 按官方分片请求示例落地为 <c>List&lt;WxaVodPartInfo&gt;</c>（裁决留档见 DTO 注释，守卫 DV4 锁定）。
    /// </para>
    /// <para><b>应答与 <see cref="SingleFileUploadAsync"/> 共用</b>：两页返回体表逐字段相同（单字段 <c>media_id</c>）⇒ 一个 <see cref="WxaVodMediaIdResponse"/>。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/commitupload")]
    Task<WxaVodMediaIdResponse> CommitUploadAsync(
        [Body] WxaVodCommitUploadRequest request,
        CancellationToken cancellationToken = default);
}

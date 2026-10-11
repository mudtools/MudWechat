// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Resource;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「资源管理」域 SDK（7 端点：图片上传（URL / 二进制双传输形态） +
/// media_id 取数 + 资质图片上传 + 视频分块上传四步流程）。
/// </summary>
/// <remarks>
/// <para>
/// <b>同路由双传输形态（官方契约，勿合并为单方法）</b>：图片上传官方页给出两种按 <c>upload_type</c>
/// 互斥的请求体传输形态——<c>upload_type=1</c> 图片 URL 模式（POST JSON 包 <c>img_url</c>）与
/// <c>upload_type=0</c> 二进制流模式（multipart/form-data 文件字段 <c>media</c>）。SDK 以
/// <see cref="IChannelsResourceService.UploadImageAsync"/>（JSON 通道）与
/// <see cref="IChannelsResourceService.UploadImageByBinaryAsync"/>（multipart 通道）两个方法承载
/// （对齐 Work 线 message/send「同路由多方法」与公众号线素材域 <c>[MultipartForm]</c> 先例），
/// 共用同一响应 DTO（裁决理由见守卫 <c>ChannelsResourceContractGuards</c> CR5）。
/// </para>
/// <para>
/// <b>media_id 与图片链接双返回形态（官方 <c>resp_type</c>）</b>：<c>resp_type=0</c> 返回
/// <c>media_id</c>/<c>pay_media_id</c>（供其他接口引用）；<c>resp_type=1</c> 返回形如
/// <c>mmecimage.cn/p/{appid}/{imgKey}</c> 的永久有效图片链接（官方原文「商品信息相关图片请务必使用此参数得到链接」）。
/// </para>
/// <para>
/// <b>media/get 二进制响应</b>：成功时返回二进制数据、报错时返回 JSON 错误信息（官方原文
/// 「请根据回包数据格式分别处理」）——SDK 以 <c>byte[]?</c> 原始响应直读（对齐 Work 线 media/get 形态），
/// 失败经组件判错器抛 <see cref="Mud.Wechat.Channels.Abstractions.Exceptions.WechatChannelsException"/>。
/// </para>
/// <para>
/// <b>视频分块上传四步流程</b>：申请上传（分配 <c>video_upload_key</c>）→ 分块上传（1M~2MB，
/// 可乱序并发）→ 完成上传（分片连续性 + SHA 校验）→ 轮询临时 URL（失效后仍不影响提交）。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Resource", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsResourceService
{
    /// <summary>
    /// 上传图片——图片 URL 模式（<c>img_upload</c>，<c>upload_type=1</c>，POST JSON 包 <c>img_url</c>）。
    /// </summary>
    /// <param name="uploadType">上传类型（官方 <c>upload_type</c>，URL 参数，必填）：本方法为图片 URL 模式，传 <see cref="ChannelsResourceImgUploadTypes.Url"/>（1）。</param>
    /// <param name="respType">返回数据类型（官方 <c>resp_type</c>，URL 参数，必填），见 <see cref="ChannelsResourceImgRespTypes"/>；商品信息相关图片官方要求务必用 <see cref="ChannelsResourceImgRespTypes.Url"/>（1）取链接。</param>
    /// <param name="request">上传请求（<c>img_url</c> 图片 URL，官方 <c>upload_type=1</c> 时必填；不支持 301/302 跳转）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片信息（<c>pic_file</c>：<c>media_id</c> / <c>pay_media_id</c> / <c>img_url</c>，按 <c>resp_type</c> 返回对应形态）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_img_upload"/></para>
    /// <para>
    /// 官方业务限制：①接口频率限制 <b>5000/min</b>；②返回的图片链接有访问频率限制（appid 维度 10000/min、
    /// imgKey 维度 100/min，超出返回 404）——<b>请勿将该图片链接用于用户端（访问量较大）展示</b>；
    /// ③仅支持 bmp、jpg(jpeg)、png、webp 格式（<c>upload_type=1</c> 时按 content-type 判断，不支持 301/302 跳转）；
    /// ④大小限制：resp_type=0 为 2MB、resp_type=1 为 10MB。
    /// </para>
    /// <para>官方错误码：<c>10020055</c>（参数有误）/ <c>10020056</c>（图片格式不合法）/ <c>10020060</c>（文件大小超出限制）/
    /// <c>10020061</c>（图片 URL 非法，如传入 mmecimage.cn/p/ 前缀的图片）/ <c>10020193</c>（不支持拉取原图 301/302 跳转）等。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/basics/img/upload")]
    Task<ChannelsResourceImgUploadResponse> UploadImageAsync(
        [Query("upload_type")] int uploadType,
        [Query("resp_type")] int respType,
        [Body] ChannelsResourceImgUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传图片——二进制流模式（<c>img_upload</c>，<c>upload_type=0</c>，multipart/form-data 文件字段 <c>media</c>）。
    /// </summary>
    /// <param name="uploadType">上传类型（官方 <c>upload_type</c>，URL 参数，必填）：本方法为二进制流模式，传 <see cref="ChannelsResourceImgUploadTypes.BinaryStream"/>（0）。</param>
    /// <param name="respType">返回数据类型（官方 <c>resp_type</c>，URL 参数，必填），见 <see cref="ChannelsResourceImgRespTypes"/>。</param>
    /// <param name="height">图片的高（官方 <c>height</c>，URL 参数，单位：像素；官方 <c>upload_type=0</c> 时必填——即本方法的必传参数）。</param>
    /// <param name="width">图片的宽（官方 <c>width</c>，URL 参数，单位：像素；官方 <c>upload_type=0</c> 时必填——即本方法的必传参数）。</param>
    /// <param name="formData">multipart 表单内容（官方表单字段名必须为 <c>media</c>，图片文件 buffer；
    /// 由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>——生成器对该标注走
    /// <see cref="IFormContent.ToHttpContentAsync"/> 通路；大小限制按 <c>resp_type</c>：0 为 2MB、1 为 10MB）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片信息（<c>pic_file</c>：<c>media_id</c> / <c>pay_media_id</c> / <c>img_url</c>，按 <c>resp_type</c> 返回对应形态）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_img_upload"/></para>
    /// <para>
    /// 官方业务限制：①接口频率限制 <b>5000/min</b>；②返回的图片链接有访问频率限制（appid 维度 10000/min、
    /// imgKey 维度 100/min，超出返回 404）——<b>请勿将该图片链接用于用户端（访问量较大）展示</b>；
    /// ③仅支持 bmp、jpg(jpeg)、png、webp 格式。
    /// </para>
    /// <para>官方错误码：<c>10020056</c>（图片格式不合法）/ <c>10020059</c>（图片为空）/ <c>10020060</c>（文件大小超出限制）等。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/basics/img/upload")]
    Task<ChannelsResourceImgUploadResponse> UploadImageByBinaryAsync(
        [Query("upload_type")] int uploadType,
        [Query("resp_type")] int respType,
        [Query("height")] int height,
        [Query("width")] int width,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过 media_id 获取数据（<c>getdatabymediaid</c>；成功时响应为二进制文件内容，报错时返回 JSON）。
    /// </summary>
    /// <param name="mediaId">媒体 id（官方 <c>media_id</c>，URL 参数，必填；来源为售后单、纠纷单等接口返回的媒体 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒体文件二进制内容（失败时由 errcode 判定器抛 <see cref="Mud.Wechat.Channels.Abstractions.Exceptions.WechatChannelsException"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_getdatabymediaid"/></para>
    /// <para>
    /// 官方契约：<b>GET</b> + URL 参数 <c>media_id</c>；官方原文「成功时返回二进制数据，报错情况下返回
    /// json 格式的错误信息，请根据回包数据格式分别处理」——SDK 以 <c>byte[]?</c> 直读原始响应，
    /// 失败经组件判错器统一抛错。
    /// </para>
    /// <para>官方业务限制：本接口无特殊注意事项；无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/channels/ec/basics/media/get")]
    Task<byte[]?> GetDataByMediaIdAsync(
        [Query("media_id")] string mediaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传资质图片（<c>qualificationupload</c>，multipart/form-data 文件字段 <c>media</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（官方表单字段名必须为 <c>media</c>，资质图片文件 file；
    /// 由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件信息（<c>data.file_id</c> 资质图片文件 id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_qualificationupload"/></para>
    /// <para>
    /// 官方业务限制：①图片上传大小限制 <b>2MB</b>；②须带 HTTP 头 <c>Content-Type: multipart/form-data</c>，
    /// POST 包请求参数需为图片二进制流、以键值对形式传参。
    /// </para>
    /// <para>官方错误码：<c>10020059</c>（图片为空）/ <c>10020060</c>（文件大小超出限制）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/basics/qualification/upload")]
    Task<ChannelsResourceQualificationUploadResponse> UploadQualificationAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请上传视频（<c>video_initupload</c>，视频分块上传第一步：分配 <c>video_upload_key</c>）。
    /// </summary>
    /// <param name="request">申请请求（<c>scene_type</c>/<c>file_type</c>/<c>file_size</c> 均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>本次上传流程关联键（<c>data.video_upload_key</c>，用于后续分块上传 / 完成上传 / 获取临时 URL）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_initupload"/></para>
    /// <para>
    /// 官方业务限制：①<b>本接口上传的视频不适用于所有场景</b>——仅当其他接口明确要求使用本接口上传视频时方可使用
    /// （官方原文）；②场景限制：售后相关视频（scene_type=173）≤ 20MB、无时长限制；商品视频（scene_type=162）
    /// ≤ 500MB 且 ≤ 180 秒。
    /// </para>
    /// <para>官方业务限制：本接口无特殊错误码（走通用错误码）；场景取值见 <see cref="ChannelsResourceVideoSceneTypes"/>。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/basics/video/initupload")]
    Task<ChannelsResourceVideoInitUploadResponse> InitVideoUploadAsync(
        [Body] ChannelsResourceVideoInitUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传视频数据分块（<c>video_uploadpart</c>，视频分块上传第二步；分块大小 1M ~ 2MB）。
    /// </summary>
    /// <param name="videoUploadKey">上传流程关联键（官方 <c>video_upload_key</c>，URL 参数，必填；通过申请上传视频接口获取）。</param>
    /// <param name="partNum">分块编号（官方 <c>partnum</c>，URL 参数，必填；最大 10000，从 1 开始）。</param>
    /// <param name="formData">multipart 表单内容（官方表单字段名必须为 <c>media</c>，视频文件分块 buffer；
    /// 由调用方构建 <see cref="IFormContent"/> 实现并标 <c>[MultipartForm]</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>服务端计算的分片 SHA1（<c>data.part_sha</c>，完成视频上传时须传回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_uploadpart"/></para>
    /// <para>
    /// 官方业务限制：①分块大小目前支持 <b>1M ~ 2MB</b>，除最后一个分块外其他分块不得小于 1M（否则调用
    /// finishupload 返回 <c>EntityTooSmall</c>）；②单个分块必须以长连接上传、<b>不支持断点续传</b>；
    /// ③分块之间<b>可乱序并发上传</b>（重入覆盖），可并发上传多个分块以提高上传速度；④分块编号从 1 开始、最大 10000。
    /// </para>
    /// <para>官方错误码：<c>10020319</c>（文件为空）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/basics/video/uploadpart")]
    Task<ChannelsResourceVideoUploadPartResponse> UploadVideoPartAsync(
        [Query("video_upload_key")] string videoUploadKey,
        [Query("partnum")] int partNum,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成视频上传（<c>video_finishupload</c>，视频分块上传第三步：校验分片连续且 SHA 匹配）。
    /// </summary>
    /// <param name="request">完成请求（<c>video_upload_key</c> 与 <c>finish_parts</c> 均官方必填；分片列表按分块偏移从小到大排列且编号连续）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（官方响应无业务字段，故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_finishupload"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；官方错误码 <c>10020342</c>（上传分块有误——除最后一个分块，
    /// 其他分块大小不得小于 1M）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/basics/video/finishupload")]
    Task<ChannelsResponse> FinishVideoUploadAsync(
        [Body] ChannelsResourceVideoFinishUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取上传后的视频临时 URL（<c>video_getplayinfo</c>，视频分块上传第四步：轮询获取）。
    /// </summary>
    /// <param name="videoUploadKey">上传流程关联键（官方 <c>video_upload_key</c>，URL 参数，必填；通过申请上传视频接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>视频临时 URL（<c>data.url</c>；临时性、会失效，但官方说明 URL 失效后仍不影响将其提交到相关接口）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_getplayinfo"/></para>
    /// <para>
    /// 官方契约：<b>GET</b> + URL 参数 <c>video_upload_key</c>；完成上传后<b>轮询</b>本接口获取视频临时 URL
    /// （视频转码期间查询会返回 10020347，请稍后再试）。
    /// </para>
    /// <para>官方错误码：<c>10020324</c>（视频不合法——可能超出文件大小或时长限制）/ <c>10020347</c>（获取视频失败——视频还在转码中）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/shop/ec/basics/video/getplayinfo")]
    Task<ChannelsResourceVideoPlayInfoResponse> GetVideoPlayInfoAsync(
        [Query("video_upload_key")] string videoUploadKey,
        CancellationToken cancellationToken = default);
}

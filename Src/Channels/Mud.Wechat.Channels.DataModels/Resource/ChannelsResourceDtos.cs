// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>
/// 上传图片（<c>img_upload</c>）图片 URL 模式（<c>upload_type=1</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体（<c>img_url</c>）；<c>upload_type</c>/<c>resp_type</c> 为 URL 参数
/// （经主包 <c>IChannelsResourceService.UploadImageAsync</c> 的 <c>[Query]</c> 参数传入，不在本 DTO 内；
/// DataModels 为叶层不反向引用主包，接口名以代码体兜底）。
/// 二进制流模式（<c>upload_type=0</c>）官方要求 formdata 文件（<c>media</c>），由
/// <c>[MultipartForm]</c> 通道承载，不使用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceImgUploadRequest
{
    /// <summary>获取或设置图片 URL（官方 <c>img_url</c>，<c>upload_type=1</c> 时必填；不支持 301/302 跳转，格式限 bmp/jpg(jpeg)/png/webp）。</summary>
    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; } = string.Empty;
}

/// <summary>
/// 上传图片（<c>img_upload</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<c>upload_type=1</c>（URL 模式）与 <c>upload_type=0</c>（二进制流模式）返回结构一致
/// ⇒ 两种传输形态共用本 DTO（裁决理由见守卫 <c>ChannelsResourceContractGuards</c> CR5）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceImgUploadResponse : ChannelsResponse
{
    /// <summary>获取或设置图片信息（官方 <c>pic_file</c>）。</summary>
    [JsonPropertyName("pic_file")]
    public ChannelsResourcePicFile? PicFile { get; set; }
}

/// <summary>图片信息（官方 <c>pic_file</c> 对象，<c>img_upload</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourcePicFile
{
    /// <summary>获取或设置 media_id（官方 <c>media_id</c>；<c>resp_type=0</c> 时返回，供其他接口使用）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置支付 media_id（官方 <c>pay_media_id</c>；<c>resp_type=0</c> 时返回）。</summary>
    [JsonPropertyName("pay_media_id")]
    public string? PayMediaId { get; set; }

    /// <summary>
    /// 获取或设置图片 URL（官方 <c>img_url</c>；<c>resp_type=1</c> 时返回，形如
    /// <c>mmecimage.cn/p/{appid}/{imgKey}</c>，永久有效；有访问频率限制，请勿用于用户端大流量展示）。
    /// </summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}

/// <summary>
/// 上传资质图片（<c>qualificationupload</c>）响应。
/// </summary>
/// <remarks>官方契约：<b>POST</b> multipart/form-data（文件字段 <c>media</c>，≤ 2MB）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceQualificationUploadResponse : ChannelsResponse
{
    /// <summary>获取或设置文件信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsResourceQualificationFile? Data { get; set; }
}

/// <summary>资质图片文件信息（官方 <c>data</c> 对象，<c>qualificationupload</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceQualificationFile
{
    /// <summary>获取或设置文件 file_id（官方 <c>file_id</c>）。</summary>
    [JsonPropertyName("file_id")]
    public string? FileId { get; set; }
}

/// <summary>申请上传视频（<c>video_initupload</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；三字段均必填。
/// 场景限制：售后相关视频（scene_type=173）≤ 20MB；商品视频（scene_type=162）≤ 500MB 且 ≤ 180 秒。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoInitUploadRequest
{
    /// <summary>获取或设置上传场景类型（官方 <c>scene_type</c>，必填，见 <see cref="ChannelsResourceVideoSceneTypes"/>）。</summary>
    [JsonPropertyName("scene_type")]
    public int SceneType { get; set; }

    /// <summary>获取或设置文件类型（官方 <c>file_type</c>，必填，示例 mp4）。</summary>
    [JsonPropertyName("file_type")]
    public string FileType { get; set; } = string.Empty;

    /// <summary>获取或设置文件大小（官方 <c>file_size</c>，必填，单位：字节）。</summary>
    [JsonPropertyName("file_size")]
    public long FileSize { get; set; }
}

/// <summary>申请上传视频（<c>video_initupload</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoInitUploadResponse : ChannelsResponse
{
    /// <summary>获取或设置上传流程信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsResourceVideoUploadKey? Data { get; set; }
}

/// <summary>视频上传流程关联键（官方 <c>data</c> 对象，<c>video_initupload</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoUploadKey
{
    /// <summary>获取或设置用于关联本次上传流程的键（官方 <c>video_upload_key</c>；分块上传 / 完成上传 / 获取临时 URL 均须传回）。</summary>
    [JsonPropertyName("video_upload_key")]
    public string? VideoUploadKey { get; set; }
}

/// <summary>上传视频数据分块（<c>video_uploadpart</c>）响应。</summary>
/// <remarks>官方契约：<b>POST</b>，URL 参数 <c>video_upload_key</c>/<c>partnum</c> + formdata 文件 <c>media</c>；分块大小 1M ~ 2MB。</remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoUploadPartResponse : ChannelsResponse
{
    /// <summary>获取或设置分片信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsResourceVideoPartSha? Data { get; set; }
}

/// <summary>视频分片校验信息（官方 <c>data</c> 对象，<c>video_uploadpart</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoPartSha
{
    /// <summary>获取或设置服务端计算的分片 SHA1（官方 <c>part_sha</c>；完成视频上传时须传回用于一致性校验）。</summary>
    [JsonPropertyName("part_sha")]
    public string? PartSha { get; set; }
}

/// <summary>完成视频上传（<c>video_finishupload</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；官方响应无业务字段（仅 errcode/errmsg，返回 <see cref="ChannelsResponse"/>）。
/// <c>finish_parts</c> 须按分块偏移从小到大排列且上传块编号连续。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoFinishUploadRequest
{
    /// <summary>获取或设置上传流程关联键（官方 <c>video_upload_key</c>，必填；通过申请上传视频接口获取）。</summary>
    [JsonPropertyName("video_upload_key")]
    public string VideoUploadKey { get; set; } = string.Empty;

    /// <summary>获取或设置文件分片列表（官方 <c>finish_parts</c>，必填；按分块偏移从小到大排列，且上传块编号连续）。</summary>
    [JsonPropertyName("finish_parts")]
    public List<ChannelsResourceVideoFinishPart> FinishParts { get; set; } = new();
}

/// <summary>已完成分片描述（官方 <c>finish_parts</c> 数组元素，<c>video_finishupload</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoFinishPart
{
    /// <summary>获取或设置分块编号（官方 <c>partnum</c>，必填，从 1 开始）。</summary>
    [JsonPropertyName("partnum")]
    public int PartNum { get; set; }

    /// <summary>获取或设置分片 SHA1（官方 <c>part_sha</c>，必填；从上传视频数据分块接口回包中获取）。</summary>
    [JsonPropertyName("part_sha")]
    public string PartSha { get; set; } = string.Empty;
}

/// <summary>获取上传后的视频临时 URL（<c>video_getplayinfo</c>）响应。</summary>
/// <remarks>官方契约：<b>GET</b> + URL 参数 <c>video_upload_key</c>；完成上传后轮询获取（转码期间返回 10020347）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoPlayInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置播放信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsResourceVideoPlayInfo? Data { get; set; }
}

/// <summary>视频播放信息（官方 <c>data</c> 对象，<c>video_getplayinfo</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceVideoPlayInfo
{
    /// <summary>获取或设置视频临时 URL（官方 <c>url</c>；临时性、会失效，官方说明 URL 失效后仍不影响将其提交到相关接口）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>图片上传类型常量（官方 <c>upload_type</c> URL 参数枚举值，<c>img_upload</c>）。</summary>
public static class ChannelsResourceImgUploadTypes
{
    /// <summary>二进制流（formdata 文件字段 <c>media</c>；此时 <c>height</c>/<c>width</c> 必填）。</summary>
    public const int BinaryStream = 0;

    /// <summary>图片 URL（不支持 301/302 跳转；请求体 <c>img_url</c>）。</summary>
    public const int Url = 1;
}

/// <summary>图片返回数据类型常量（官方 <c>resp_type</c> URL 参数枚举值，<c>img_upload</c>）。</summary>
public static class ChannelsResourceImgRespTypes
{
    /// <summary>media_id 和 pay_media_id（大小限制 2MB）。</summary>
    public const int MediaId = 0;

    /// <summary>图片链接（形如 mmecimage.cn/p/{appid}/{imgKey}，永久有效；官方原文「商品信息相关图片请务必使用此参数得到链接」；大小限制 10MB）。</summary>
    public const int Url = 1;
}

/// <summary>视频上传场景类型常量（官方 <c>scene_type</c> 枚举值，<c>video_initupload</c>）。</summary>
public static class ChannelsResourceVideoSceneTypes
{
    /// <summary>售后相关的视频（大小 ≤ 20MB，无时长限制）。</summary>
    public const int Aftersale = 173;

    /// <summary>商品视频（大小 ≤ 500MB，时长 ≤ 180 秒）。</summary>
    public const int Product = 162;
}

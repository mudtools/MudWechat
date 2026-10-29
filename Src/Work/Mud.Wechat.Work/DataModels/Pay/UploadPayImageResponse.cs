// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 提交图片响应体（<c>/cgi-bin/miniapppay/upload_image</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class UploadPayImageResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置上传后得到的图片 ID（30 天后过期；当所有用到该图片的申请单
    /// 都完成流程后图片 ID 会立即失效）。
    /// </summary>
    [JsonPropertyName("open_wx_pay_media_id")]
    public string? OpenWxPayMediaId { get; set; }
}

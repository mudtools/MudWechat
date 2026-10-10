// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 查询注册状态请求体（<c>/cgi-bin/service/get_register_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class GetRegisterInfoRequest
{
    /// <summary>
    /// 获取或设置查询的注册码（官方必填）。
    /// <para>官方限制：<c>register_code</c> 生成后的查询有效期为 24 小时。</para>
    /// <para>官方限制：仅支持「注册完成回调事件」或「获取注册码」接口返回的 <c>register_code</c> 调用。</para>
    /// </summary>
    [JsonPropertyName("register_code")]
    public string RegisterCode { get; set; } = string.Empty;
}

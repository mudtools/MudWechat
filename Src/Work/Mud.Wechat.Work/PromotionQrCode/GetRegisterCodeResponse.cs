// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 获取注册码响应体（<c>/cgi-bin/service/get_register_code</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class GetRegisterCodeResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置注册码（最长为 512 个字节）。
    /// <para>官方限制：只能消费一次，在访问注册链接时消费。</para>
    /// </summary>
    [JsonPropertyName("register_code")]
    public string RegisterCode { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置注册码的有效期（秒）。
    /// <para>官方说明：生成链接需要在有效期内点击跳转（官方示例值为 600）。</para>
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

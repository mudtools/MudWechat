// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送骑手信息（<c>agent</c> 内嵌对象；运力方更新状态时骑手接单需返回）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAgent
{
    /// <summary>骑手姓名（<c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>骑手电话（<c>phone</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }

    /// <summary>电话是否加密（<c>is_phone_encrypted</c>，可选；默认 <c>0</c> 不加密，<c>1</c> 加密）。</summary>
    [JsonPropertyName("is_phone_encrypted")]
    public int? IsPhoneEncrypted { get; set; }
}

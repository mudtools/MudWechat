// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>用户手机状态查询请求体（<c>POST /cgi-bin/express/delivery/userquery</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliveryuserquery"/>
/// （接口英文名 <c>deliveryUserQuery</c>）。运力方调用本接口查询某手机号是否已绑定。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUserQueryRequest
{
    /// <summary>手机号（<c>phone</c>，必填）。</summary>
    [JsonPropertyName("phone")]
    public string? PhoneNumber { get; set; }
}

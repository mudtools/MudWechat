// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>用户手机状态查询响应（<c>POST /cgi-bin/express/delivery/userquery</c>）。</summary>
/// <remarks>错误码口径与 <see cref="WxaExpressPathNotifyResponse"/> 相同（<c>150004</c> 运力配置不存在、<c>932001</c>~<c>932003</c> 反查运单异常）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUserQueryResponse : WxaResponse
{
    /// <summary>用户是否绑定（<c>exist</c>；<c>0</c> 未绑定，<c>1</c> 已绑定）。</summary>
    [JsonPropertyName("exist")]
    public int Exist { get; set; }
}

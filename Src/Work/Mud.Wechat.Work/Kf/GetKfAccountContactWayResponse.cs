// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取客服账号链接响应体（<c>/cgi-bin/kf/add_contact_way</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfAccountContactWayResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置客服链接（可嵌入 H5 页面供用户点击发起咨询，也可据此自行生成二维码）。
    /// <para>
    /// 返回的客服链接不能修改或复制参数到其他链接使用，否则进入会话事件参数校验不通过，导致无法回调。
    /// </para>
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

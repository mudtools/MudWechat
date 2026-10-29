// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Contracts;

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 获取用户基本信息响应（<c>userInfo</c>，<c>GET /cgi-bin/user/info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何继承 <see cref="MpUserInfo"/> 而非 <see cref="MpResponse"/></b>：官方本端点响应是
/// 「用户字段<b>平铺</b> + 出错时附带 <c>errcode</c>/<c>errmsg</c>」——成功与失败共用同一层级。
/// 若改为 <c>: MpResponse</c> + 重新声明全部用户字段，会与
/// <see cref="MpUserInfo"/>（批量端点的元素类型）产生<b>两份会漂移的字段声明</b>。
/// 故这里以「用户字段继承 + 判错契约显式实现」表达官方形态。
/// </para>
/// <para>
/// 官方响应字段（逐项核验）：<c>subscribe</c> / <c>openid</c> / <c>language</c> / <c>subscribe_time</c> /
/// <c>unionid</c> / <c>remark</c> / <c>groupid</c> / <c>tagid_list</c> / <c>subscribe_scene</c> /
/// <c>qr_scene</c> / <c>qr_scene_str</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpUserInfoResponse : MpUserInfo, IWechatApiResponse
{
    /// <summary>业务错误码（成功响应缺省 ⇒ 0）。</summary>
    [JsonPropertyName("errcode")]
    public virtual int ErrorCode { get; set; }

    /// <summary>业务错误信息（可空）。</summary>
    [JsonPropertyName("errmsg")]
    public virtual string? ErrorMessage { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsSuccess => ErrorCode == 0;
}

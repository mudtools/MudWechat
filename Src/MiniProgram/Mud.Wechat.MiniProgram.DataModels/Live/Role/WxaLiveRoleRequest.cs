// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 「微信号 + 角色」请求体（<c>POST /wxaapi/broadcast/role/{addrole,deleterole}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：设置成员角色与移除成员角色两端的官方请求体字段集完全一致
/// （<c>username</c> + <c>role</c>，均必填）⇒ 共用本类型。两者应答不同：addrole 可能回
/// <c>codeurl</c>（见 <see cref="WxaLiveAddRoleResponse"/>），deleterole 只回 <c>errcode</c> / <c>errmsg</c>。
/// </para>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelistdw.html"/>。
/// </para>
/// <para>
/// <b>官方页面别名异常（留档）</b>：addrole 的官方页面文件名为 <c>api_getrolelistdw</c>、
/// 接口英文名标为 <c>getRoleListdw</c>（与「查询成员列表」页 <c>api_getrolelist</c> 混淆），
/// 而请求体、云调用（<c>liveBroadcast.addRole</c>）与错误码族（400001~400004）均为「设置角色」语义
/// ⇒ 本 SDK 按路由与请求体建模，页面别名不进入类型名。
/// </para>
/// <para>
/// <b>数量上限（官方错误码原文）</b>：<c>400003</c>「添加角色达到上限（管理员 10 个，运营者 500 个，主播 500 个）」；
/// <c>400004</c>「重复添加角色」；<c>400005</c>（删除侧）「主播角色删除失败，该主播存在未开播的直播间」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoleRequest
{
    /// <summary>用户的微信号（<c>username</c>，必填）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>用户角色（<c>role</c>，必填），取值见 <see cref="WxaLiveRoleType"/>。</summary>
    [JsonPropertyName("role")]
    public long? Role { get; set; }
}

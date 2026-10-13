// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 成员角色取值（<c>role</c> 请求参数与 <c>roleList[]</c> 应答元素，官方枚举）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelist.html"/>。
/// 官方原文「[0-超级管理员，1-管理员，2-主播，3-运营者]」⇒ 四值。
/// 数量上限由官方错误码 <c>400003</c> 给出（管理员 10 个、运营者 500 个、主播 500 个），
/// 超级管理员不在上限描述内 ⇒ SDK 不本地校验，照录官方返回码。
/// </remarks>
public static class WxaLiveRoleType
{
    /// <summary>超级管理员（<c>0</c>）。</summary>
    public const int SuperAdmin = 0;

    /// <summary>管理员（<c>1</c>，官方上限 10 个）。</summary>
    public const int Admin = 1;

    /// <summary>主播（<c>2</c>，官方上限 500 个；须已实名认证，否则 <c>400002</c>）。</summary>
    public const int Anchor = 2;

    /// <summary>运营者（<c>3</c>，官方上限 500 个）。</summary>
    public const int Operator = 3;
}

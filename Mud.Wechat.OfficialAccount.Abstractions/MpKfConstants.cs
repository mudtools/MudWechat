// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 在线状态取值常量（官方 <c>getonlinekflist</c> 的 <c>status</c> 字段）。
/// </summary>
/// <remarks>
/// <b>为何放在 Abstractions 而非 DataModels</b>：数据模型命名空间被契约守卫按
/// 「该命名空间下每个类型都必须带 <c>[HttpJsonSerializable]</c>」整体扫描（防漏登记 AOT 上下文）
/// ⇒ 无序列化语义的常量类不得混入，否则守卫要么放行漏登记、要么对常量类误报。
/// </remarks>
public static class MpKfOnlineStatus
{
    /// <summary>不在线。</summary>
    public const int Offline = 0;

    /// <summary>web 在线。</summary>
    public const int WebOnline = 1;
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 小程序交易类型（<c>setwxatradetypecgi.trade_type</c>，官方枚举表 1~4 连续）。
/// </summary>
/// <remarks>
/// <para><b>这是「申请变更后的目标类型」而非当前类型</b>：本接口不返回交易类型，也无查询伴侣端点
/// （官方 order_shipping 目录内无对应页面）⇒ 变更结果需走小程序后台或客服确认。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setwxatradetypecgi.html"/></para>
/// </remarks>
public static class WxaOrderTradeTypeValues
{
    /// <summary>综合类（<c>1</c>）。</summary>
    public const int Comprehensive = 1;

    /// <summary>实物电商类（<c>2</c>）。</summary>
    public const int PhysicalECommerce = 2;

    /// <summary>线下服务类（<c>3</c>）。</summary>
    public const int OfflineService = 3;

    /// <summary>在线服务类（<c>4</c>）。</summary>
    public const int OnlineService = 4;
}

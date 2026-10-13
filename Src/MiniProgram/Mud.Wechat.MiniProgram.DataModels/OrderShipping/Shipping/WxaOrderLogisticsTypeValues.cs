// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 物流模式（<c>logistics_type</c>，上传侧与查询侧同一取值表，1~4 连续）。
/// </summary>
/// <remarks>
/// <para><b>只有 <see cref="PhysicalLogistics"/>（1）允许调用「确认收货提醒」</b>（官方注意事项
/// 「只有物流类型为物流快递时才能进行提醒」）。</para>
/// <para><b>官方原文在 3 的说明里重复了一次「虚拟商品」</b>（照录不改）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
public static class WxaOrderLogisticsTypeValues
{
    /// <summary>实体物流配送（<c>1</c>）：采用快递公司进行实体物流配送形式。</summary>
    public const int PhysicalLogistics = 1;

    /// <summary>同城配送（<c>2</c>）。</summary>
    public const int IntraCityDelivery = 2;

    /// <summary>虚拟商品（<c>3</c>）：例如话费充值、点卡等，无实体配送形式。</summary>
    public const int VirtualGoods = 3;

    /// <summary>用户自提（<c>4</c>）。</summary>
    public const int UserSelfPickup = 4;
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 价格类型取值（<c>priceType</c> / 应答侧 <c>price_type</c>，官方枚举）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_addgoods.html"/>。
/// 官方原文三值口径：<c>1</c> 一口价（只传 price，price2 不传）/ <c>2</c> 价格区间（price 为左边界、price2 为右边界，两者必传）/
/// <c>3</c> 显示折扣价（price 为原价、price2 为现价，两者必传）⇒ <b>非 1 时 price2 必传</b>属官方业务约束，
/// SDK 不做本地校验（照录原则），由官方 <c>300003</c>「价格输入不合规」兜住。
/// </remarks>
public static class WxaLivePriceType
{
    /// <summary>一口价：只需传入 price，price2 不传（<c>1</c>）。</summary>
    public const int Fixed = 1;

    /// <summary>价格区间：price 为左边界、price2 为右边界，两者必传（<c>2</c>）。</summary>
    public const int Range = 2;

    /// <summary>显示折扣价：price 为原价、price2 为现价，两者必传（<c>3</c>）。</summary>
    public const int Discount = 3;
}

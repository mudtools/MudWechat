// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 品牌类型（<c>Application.audit_info.brand_type</c>，官方枚举表 1~8 连续无缺号）。
/// </summary>
/// <remarks>
/// <para><b>取值决定条件必填面</b>：<c>4</c>（电商平台旗舰店）要求
/// <c>flagship_in_which_ec_platform</c> + <c>ec_platform_proof_list</c>；
/// <c>7</c>（驰名商标、著名商标）要求 <c>authority_certified_proof_list</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html"/></para>
/// </remarks>
public static class WxaFamousBrandTypeValues
{
    /// <summary>工信部消费品名单（<c>1</c>）。</summary>
    public const int IndustryConsumerList = 1;

    /// <summary>中国连锁经营协会网络零售（<c>2</c>）。</summary>
    public const int CcfaOnlineRetail = 2;

    /// <summary>中华老字号（<c>3</c>）。</summary>
    public const int ChinaTimeHonoredBrand = 3;

    /// <summary>电商平台旗舰店（<c>4</c>）。</summary>
    public const int ECommerceFlagshipStore = 4;

    /// <summary>全球500强企业品牌（<c>5</c>）。</summary>
    public const int GlobalFortune500Brand = 5;

    /// <summary>中国500强企业经营品牌（<c>6</c>）。</summary>
    public const int ChinaFortune500Brand = 6;

    /// <summary>有关部门认定的驰名商标、著名商标（<c>7</c>）。</summary>
    public const int AuthorityCertifiedTrademark = 7;

    /// <summary>工信部消费名品成长企业名单（<c>8</c>）。</summary>
    public const int IndustryGrowingBrandList = 8;
}

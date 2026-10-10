// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Extensions;

/// <summary>
/// 微信小店 / 视频号（channels 生态）API 模块枚举（对齐公众号 <c>MpModule</c>；三段式注册第一段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>成员即 <c>RegistryGroupName</c></b>：与 <c>[HttpClientApi(RegistryGroupName = …)]</c> 同名，
/// 源生成器据此产出 <c>Add{域}WebApiHttpClient()</c>。
/// </para>
/// <para>
/// <b>令牌签发不经本枚举</b>：<c>token</c>/<c>stable_token</c> 属本线令牌基座
/// （<c>AddChannelsApp</c> 自动注册），故本枚举只列业务域（设计方案 v1 §2.2）。
/// </para>
/// <para>
/// <b>27 个业务域的计数口径</b>：逐域端点计数为<b>规划口径</b>（对应 318 端点规划总数），
/// 权威口径以 <c>ChannelsRouteContractGuards</c>（CH-R2）与 P1 起逐域守卫为准；
/// <c>/wxa/</c> 6 个「小程序会员服务」端点暂缓落位（守卫 CH-V1），不计入任何域。
/// </para>
/// </remarks>
public enum ChannelsModule
{
    /// <summary>
    /// 基础接口（8 端点：quota/get、rid/get、clear_quota、quota/clear、clear_quota/v2（免令牌）、
    /// callback/check、get_api_domain_ip、getcallbackip）。
    /// </summary>
    /// <remarks>与公众号线云端同路由（设计方案 v1 §4.5），本线自建、各自消费各自令牌。</remarks>
    Basic,

    /// <summary>资源管理（7 端点：图片 / 资质 / 视频分块上传，<c>/shop/ec/basics/*</c>）。</summary>
    Resource,

    /// <summary>店铺管理（4 端点：店铺信息 / H5 链接 / 二维码 / 口令）。</summary>
    Shop,

    /// <summary>主页管理（14 端点：商品排序 / 背景图 / 精选 / 分类）。</summary>
    HomePage,

    /// <summary>商品管理（43 端点：商品 / 库存 / 赠品 / 买赠活动 / 限时抢购）。</summary>
    Product,

    /// <summary>收藏管理（1 端点：收藏数统计）。</summary>
    Favorite,

    /// <summary>类目管理（11 端点：类目 8 + 类目规则 3）。</summary>
    Category,

    /// <summary>订单管理（27 端点）。</summary>
    Order,

    /// <summary>资金结算（16 端点：余额 / 结算账户 / 提现 / 流水 / 银行·城市查询 / 资金二维码；<b>非</b>支付收单）。</summary>
    Funds,

    /// <summary>营销管理（7 端点：优惠券）。</summary>
    Marketing,

    /// <summary>售后管理（27 端点：售后单 / 纠纷单 / 保障单）。</summary>
    Aftersale,

    /// <summary>商家客服（2 端点：cos 上传 + 发送消息）。</summary>
    Kf,

    /// <summary>质检管理（5 端点：质检仓 / 送检 / 质检码 / 自寄送检）。</summary>
    Qic,

    /// <summary>物流发货（28 端点：地址 / 运费模板 / 电子面单 / 发货）。</summary>
    Logistics,

    /// <summary>区域仓库（11 端点）。</summary>
    Warehouse,

    /// <summary>优选联盟（11 端点：带货者 / 商品）。</summary>
    League,

    /// <summary>品牌资质（8 端点）。</summary>
    Brand,

    /// <summary>代发与供货（16 端点：代发管理 13 + 供货管理 3）。</summary>
    Delivery,

    /// <summary>企业微信关联（1 端点：获取关联账号企微 id）。</summary>
    Wecom,

    /// <summary>连接小程序（10 端点：基础 2 + 授权送礼 8）。</summary>
    MiniStore,

    /// <summary>罗盘（14 端点：商家版 10 + 达人版 4）。</summary>
    Compass,

    /// <summary>会员营销（4 端点）。</summary>
    Vip,

    /// <summary>直播与留资（12 端点：直播记录 / 留资组件 / 留资数据 / 直播大屏，视频号内容面）。</summary>
    Live,

    /// <summary>橱窗与本地生活商品（13 端点：橱窗 4 + 本地生活商品 9）。</summary>
    Window,

    /// <summary>本地生活（8 端点：团购券 / 核销 / 撤销 / 券账单，<c>/channels/ec/voucher/*</c>）。</summary>
    Locallife,

    /// <summary>国补管理（3 端点）。</summary>
    Subsidy,

    /// <summary>客诉工单（4 端点）。</summary>
    PlatformKf,
}
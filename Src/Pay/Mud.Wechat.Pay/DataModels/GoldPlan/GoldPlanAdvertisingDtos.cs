// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.GoldPlan;

/// <summary>关闭广告展示请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473781"/></summary>
[HttpJsonSerializable(SerializerClassName = "GoldPlan")]
public class GoldPlanAdvertisingShowCloseRequest
{
    /// <summary>【特约商户号】 需要关闭广告展示的特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>开通广告展示请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473794"/></summary>
[HttpJsonSerializable(SerializerClassName = "GoldPlan")]
public class GoldPlanAdvertisingShowOpenRequest
{
    /// <summary>【特约商户号】 需要开通广告展示的特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【同业过滤标签值】 特约商户同业过滤的同业过滤标签值。如已设置同业过滤标签，再次请求传入，视为新增，将覆盖原有同业标签配置。可选取值：`E_COMMERCE` 综合电商平台 / `LOVE_MARRIAGE` 婚恋 / `POTOGRAPHY` 摄影摄像机构及服务 / `EDUCATION` 教育 / `FINANCE` 金融 / `TOURISM` 旅游 / `SKINCARE` 护肤彩妆 / `FOOD` 食品 / `SPORT` 运动户外 / `JEWELRY_WATCH` 珠宝钟表 / `HEALTHCARE` 医疗健康 / `BUSSINESS` 商务服务 / `PARENTING` 亲子 / `CATERING` 餐饮美食 / `RETAIL` 零售百货 / `SERVICES` 生活服务 / `LAW` 法律服务 / `ESTATE` 房地产 / `TRANSPORTATION` 交通运输 / `ENERGY_SAVING` 节能环保 / `SECURITY` 安全安保 / `BUILDING_MATERIAL` 家居装修建材 / `COMMUNICATION` 通讯及IT服务 / `MERCHANDISE` 日用百货 / `ASSOCIATION` 机构协会 / `COMMUNITY` 网络资讯及社区 / `ONLINE_AVR` 在线视听与阅读 / `WE_MEDIA` 自媒体 / `CAR` 汽车 / `SOFTWARE` 软件工具 / `GAME` 游戏 / `CLOTHING` 服饰鞋帽箱包 / `INDUSTY` 工业工程 / `AGRICULTURE` 农林牧渔 / `PUBLISHING_MEDIA` 出版传媒 / `HOME_DIGITAL` 数码家电（官方字段路径 <c>advertising_industry_filters</c>，选填，类型 <c>array[IndustryType]</c>）。</summary>
    [JsonPropertyName("advertising_industry_filters")]
    public List<string>? AdvertisingIndustryFilters { get; set; }

}

/// <summary>同业过滤标签管理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473784"/></summary>
[HttpJsonSerializable(SerializerClassName = "GoldPlan")]
public class GoldPlanIndustryFilterSetRequest
{
    /// <summary>【特约商户号】 需要设置“同业过滤标签”的特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【同业过滤标签值】 特约商户同业过滤的同业过滤标签值，同业过滤标签最少传一个，最多三个。如已设置同业过滤标签，再次请求传入，视为新增，将覆盖原有同业标签配置。 注：若配置完成后需清空标签的，可登陆商户平台手动清空（路径：商户平台-&gt;服务商功能-&gt;点金计划-&gt;特约商户管理-&gt;同业过滤标签）。可选取值：`E_COMMERCE` 综合电商平台 / `LOVE_MARRIAGE` 婚恋 / `POTOGRAPHY` 摄影摄像机构及服务 / `EDUCATION` 教育 / `FINANCE` 金融 / `TOURISM` 旅游 / `SKINCARE` 护肤彩妆 / `FOOD` 食品 / `SPORT` 运动户外 / `JEWELRY_WATCH` 珠宝钟表 / `HEALTHCARE` 医疗健康 / `BUSSINESS` 商务服务 / `PARENTING` 亲子 / `CATERING` 餐饮美食 / `RETAIL` 零售百货 / `SERVICES` 生活服务 / `LAW` 法律服务 / `ESTATE` 房地产 / `TRANSPORTATION` 交通运输 / `ENERGY_SAVING` 节能环保 / `SECURITY` 安全安保 / `BUILDING_MATERIAL` 家居装修建材 / `COMMUNICATION` 通讯及IT服务 / `MERCHANDISE` 日用百货 / `ASSOCIATION` 机构协会 / `COMMUNITY` 网络资讯及社区 / `ONLINE_AVR` 在线视听与阅读 / `WE_MEDIA` 自媒体 / `CAR` 汽车 / `SOFTWARE` 软件工具 / `GAME` 游戏 / `CLOTHING` 服饰鞋帽箱包 / `INDUSTY` 工业工程 / `AGRICULTURE` 农林牧渔 / `PUBLISHING_MEDIA` 出版传媒 / `HOME_DIGITAL` 数码家电（官方字段路径 <c>advertising_industry_filters</c>，<b>必填</b>，类型 <c>array[IndustryType]</c>）。</summary>
    [JsonPropertyName("advertising_industry_filters")]
    public List<string>? AdvertisingIndustryFilters { get; set; }

}

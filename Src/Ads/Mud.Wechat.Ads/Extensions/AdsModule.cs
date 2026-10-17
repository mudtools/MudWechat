// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Extensions;

/// <summary>
/// 腾讯广告（Marketing API v3.0）业务模块枚举（三段式注册的第一段，形态照 <c>PayModule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>成员即源生成器的 <c>RegistryGroupName</c></b>：每个成员都对应各接口上
/// <c>[HttpClientApi(RegistryGroupName = "…")]</c> 与生成产出的 <c>Add{成员}WebApiHttpClient()</c>。
/// </para>
/// <para>
/// <b>成员只列已落地域</b>：没有对应注册方法的成员会落进 <see cref="AdsServiceBuilder.AddModules"/>
/// 的「查不到注册器 ⇒ 静默不注册」分支，是无声陷阱（与 <c>PayModule</c> 同一处置）。
/// </para>
/// <para>模块注册三段式：本枚举值 + <c>Add{域}Api()</c>（见 <see cref="AdsServiceBuilder"/>）+ 源生成器产出的 <c>Add{域}WebApiHttpClient()</c>。</para>
/// </remarks>
public enum AdsModule
{
    /// <summary>
    /// 客户账号（3 端点）：查询客户信息 + 更新客户信息 + 批量修改账户日预算。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/get"/>。
    /// </summary>
    Advertiser,

    /// <summary>
    /// 营销单元（8 端点）：查询 / 创建 / 更新 / 删除 + 四支批量（日预算、投放状态、出价、投放日期与时段）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/adgroups/get"/>。
    /// </summary>
    /// <remarks>
    /// 组名取 <c>Adgroups</c>（照官方资源族 <c>adgroups/*</c> 原文复数形态，与接口上的
    /// <c>[HttpClientApi(RegistryGroupName = "Adgroups")]</c> 同源；守卫 ADS-B2 的三段式一致性断言负责钉住）。
    /// </remarks>
    Adgroups,

    /// <summary>
    /// 报表（4 端点，跨 <c>daily_reports</c> / <c>hourly_reports</c> / <c>async_reports</c> 三支官方资源族）：
    /// 日报表 + 小时报表 + 创建异步报表任务 + 查询异步报表任务。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/daily_reports/get"/>。
    /// </summary>
    /// <remarks>
    /// <b>一族一模块的例外，理由是链路与权限同构</b>：三支资源族的权限都是 <c>ads_insights</c>、
    /// 信封与分页形态同构，且异步链路的语义是一体的（<c>add</c> 拿 <c>task_id</c> → <c>get</c> 读
    /// <c>status</c> 与 <c>file_id</c>）⇒ 拆成三个模块只会得到三个空注册面。
    /// <b>族级差异全部留在端点面</b>（<c>level</c> 三支集合互不相同、hourly 无 <c>organization_id</c>、
    /// 分页上限逐页不同），由 <c>IWechatAdsReportService</c> 的逐方法 XML 与守卫 ADS-B2 锁定，
    /// 不因合并模块而抹平。
    /// </remarks>
    Reports,

    /// <summary>
    /// 组件化创意（4 端点）：查询 / 创建 / 更新 / 删除（官方 <c>dynamic_creatives/*</c>，v3.0 创意核心）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/dynamic_creatives/get"/>。
    /// </summary>
    /// <remarks>
    /// 三支写端点为受限接口（另列 <c>user_token</c>，模块注册期已登记强制掩码键，守卫 ADS-B5）。
    /// 组件 <c>value</c> 为逐组件 union，以开放字典承载（裁剪决策见核验留档 §4-D1）。
    /// </remarks>
    DynamicCreatives,

    /// <summary>
    /// 创意组件（4 端点）：查询 / 创建 / 删除 + 组件详情（官方 <c>components/*</c> + <c>component_detail/get</c>）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/components/get"/>。
    /// </summary>
    Components,

    /// <summary>
    /// 图片素材（4 端点）：查询 / 修改描述 / 删除（声明式）+ 上传（<c>images/add</c> 为 multipart/form-data，
    /// 走手写通道 <c>IWechatAdsImageUploadService</c>，随本模块装配）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/images/get"/>。
    /// </summary>
    Images,

    /// <summary>
    /// 视频素材（4 端点）：查询 / 修改描述 / 删除（声明式）+ 上传（<c>videos/add</c> 为 multipart/form-data，
    /// 走手写通道 <c>IWechatAdsVideoUploadService</c>，随本模块装配）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/videos/get"/>。
    /// </summary>
    Videos,

    /// <summary>
    /// 异步任务（2 端点）：创建 + 查询（官方 <c>async_tasks/add|get</c>；双层判定 ——
    /// 任务执行结果看 <c>result.code</c>）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/async_tasks/add"/>。
    /// </summary>
    AsyncTasks,

    /// <summary>
    /// 人群管理（5 端点）：创建 / 查询 / 修改客户人群（<c>custom_audiences/*</c>）+ 人群数据文件
    /// 查询（<c>custom_audience_files/get</c>，声明式）与上传（<c>custom_audience_files/add</c> 为
    /// multipart/form-data，走手写通道 <c>IWechatAdsCustomAudienceFileUploadService</c>，随本模块装配）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/custom_audiences/add"/>。
    /// </summary>
    /// <remarks>
    /// 一族一模块的口径同 <see cref="Reports"/>：audiences 与 files 两支资源族权限同一
    /// （<c>audience_management,ads_management,user_actions</c>）、链路语义一体（创建人群 → 上传号码包 → 查询）。
    /// 官方另有 <c>custom_audiences/delete</c>，本 SDK 暂未消费。
    /// </remarks>
    CustomAudiences,

    /// <summary>
    /// 资金账户（4 端点，跨 <c>funds</c> / <c>fund_transfer</c> / <c>fund_statements_detailed</c>
    /// / <c>realtime_cost</c> 四支官方资源族）：查询资金信息 + 发起转账 + 查询资金流水明细 + 获取账号实时消耗。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/funds/get"/>。
    /// </summary>
    /// <remarks>
    /// 一族一模块的口径同 <see cref="Reports"/>：四族权限同一（<c>account_management</c>）、链路语义一体
    /// （查资金 → 转账 → 拉流水 → 查实时消耗）⇒ 拆成四个模块只会得到四个空注册面。<b>逐页差异留在端点面</b>
    /// （<c>fund_type</c> 转账页 9 支、流水页 36 支互不相同；<c>realtime_cost</c> 页 <c>level</c> 三值
    /// 无 <c>REPORT_LEVEL_</c> 前缀，由守卫 ADS-B2 与接口 XML 锁定）。
    /// </remarks>
    Funds,

    /// <summary>
    /// 客户资质（3 端点）：获取 / 创建 / 更新客户资质（官方 <c>qualifications/*</c>）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/qualifications/get"/>。
    /// </summary>
    /// <remarks>
    /// 三页权限同一 <c>account_management</c>、<c>account_id</c> 口径「不支持代理商 id」三页一致。
    /// 官方另有 <c>qualifications/delete</c>，本 SDK 暂未消费（同 <see cref="CustomAudiences"/> 处置）。
    /// </remarks>
    Qualifications,

    /// <summary>
    /// 广告预估（1 端点）：预估覆盖人数（官方 <c>estimation/get</c>）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/estimation/get"/>。
    /// </summary>
    /// <remarks>
    /// 权限 <c>ads_management</c>（与 <see cref="Funds"/> / <see cref="Qualifications"/> 的
    /// <c>account_management</c> 不同权限族 ⇒ 独立成域）。请求体的定向/场景/载体子树复用
    /// <c>Adgroups</c> 域类型（2026-10-17 逐字段比对同构），应答为单结构体的区间与建议值。
    /// </remarks>
    Estimation,

    /// <summary>
    /// 定向标签（1 端点）：获取定向标签（官方 <c>targeting_tags/get</c>）。
    /// 官方文档 <see href="https://developers.e.qq.com/v3.0/docs/api/targeting_tags/get"/>。
    /// </summary>
    /// <remarks>
    /// 权限 <c>ads_management</c>。本域是投放定向取值（<c>regions</c> / <c>business_districts</c> /
    /// 设备品牌型号等）的供给侧，应答 <c>id</c> 回填 <c>adgroups</c> 定向参数。
    /// </remarks>
    TargetingTags,
}

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
}

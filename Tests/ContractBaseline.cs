// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.ContractBaseline;

/// <summary>
/// 契约<b>聚合基线</b>：跨域 / 跨族的<b>汇总</b>数字，集中一处维护。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么集中</b>：这些数字不描述任何单一域的契约，只承担「有没有东西悄悄多出来 / 少掉」的
/// 漂移报警。散落在各守卫里会让「加一个端点 / 一组接口」变成跨 N 个文件的机械改动，
/// 且改一处漏一处时不报错。集中后：<b>改数字只改本文件一处</b>，各守卫只断言
/// 「现场推导值 == 基线值」。
/// </para>
/// <para>
/// <b>边界（勿越界收纳）</b>：<i>域内</i>的精确断言（某域的端点数、某族的 DTO 数、某域的类型数等）
/// <b>不</b>入本文件 —— 按 AGENTS §2「守卫是权威描述」，那些是领域契约的一部分，
/// 应留在对应域守卫里就近维护。本文件只收「跨域 / 跨族汇总」的报警数。
/// </para>
/// <para>
/// <b>接线</b>：本文件位于 <c>Tests/</c> 根下、不属任何工程的默认 glob，故由
/// 四个产品线测试工程以 <c>&lt;Compile Include="..\ContractBaseline.cs" Link="ContractBaseline.cs" /&gt;</c>
/// 显式链接（各工程各得一份同名类型的副本，互不影响），并在各自 <c>GlobalUsings.cs</c> 中
/// <c>global using Mud.Wechat.ContractBaseline;</c>。
/// </para>
/// <para>
/// <b>改这里的正确姿势</b>：先核对官方开放面 / 台账，再改本文件<b>一处</b>；随后跑对应守卫 ——
/// 断言消息会指向本文件，故漂移必红。禁止在守卫里写回字面量。
/// </para>
/// </remarks>
public static class Baseline
{
    /// <summary>企业微信线（`Tests/Mud.Wechat.Work.Tests`）。</summary>
    public static class Work
    {
        /// <summary>
        /// 公共父接口数：`[HttpClientApi]` 且 `IsAbstract = true`（不注册 DI，仅作继承基座）。
        /// </summary>
        /// <remarks>锁定于 `WechatInterfaceNamespaceContractGuards`（N1）。改此值须先核对官方开放面。</remarks>
        public const int AbstractParentInterfaces = 147;

        /// <summary>
        /// 可注入接口数：`[HttpClientApi]` 且非抽象（应用类型子接口 + 独立端点接口）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatInterfaceNamespaceContractGuards`（N1）。
        /// 与 <see cref="AbstractParentInterfaces"/> 合计即契约面接口总数（当前 446）。
        /// </remarks>
        public const int InjectableInterfaces = 299;

        /// <summary>
        /// 已登记 `[WechatCallbackContract]` 的事件键总数（常量声明与特性声明的**并集**）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatCallbackContractGuards`（CB 系列）。
        /// 现行推导：17 + 安全管理 1 + 微信客服 1 + 客户联系/获客族 5 + 90240 的 24 + 应用版本付费订单回调族 6
        /// + 接口调用许可族 4 + 邮箱族 2 + 文档族 5 + 智能表格族 6 + 日程族 5 + 会议族 30 + 家校沟通族 2
        /// + 会话内容存档 3 + 微盘族 9 + 直播族 1 + OA 审批族 1 = 122。
        /// </remarks>
        public const int RegisteredCallbackEventKeys = 122;
    }

    /// <summary>公众号线（`Tests/Mud.Wechat.OfficialAccount.Tests`）。</summary>
    public static class OfficialAccount
    {
        /// <summary>
        /// 主接口（`[HttpClientApi]`）路由数：**同一路由的多方法形态只计 1 条**（如 OCR 的 `*ByUpload` / `*ByUrl` 双形态）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `MpRouteCountGuard`（RC1）。
        /// 现行推导：P4 已完成域（Store 12 + OneCode 6 + Invoice 17）与卡券域（14）之后，
        /// 127 − 6 非特性路由 + 18 + 12 + 6 + 17 + 14 = 188。
        /// </remarks>
        public const int MainInterfaceRoutes = 188;

        /// <summary>
        /// 全量去重路由数（主接口 + 认证/票据基座 + 非 JSON 下载通道）。
        /// </summary>
        /// <remarks>锁定于 `MpRouteCountGuard`（RC2）。现行推导：188 + AbstractsAuth 3 + 下载通道 3 = 194。</remarks>
        public const int TotalRoutes = 194;

        /// <summary>
        /// 官方面唯一路由数（索引页表格取值并集口径）。
        /// </summary>
        /// <remarks>锁定于 `MpRouteCountGuard`（RC3）。与 `.docs/公众号服务号/` 台账逐条比对；fresh clone 无 `.docs` 时该断言跳过。</remarks>
        public const int OfficialRoutes = 196;
    }

    /// <summary>小程序线（`Tests/Mud.Wechat.MiniProgram.Tests`）。</summary>
    public static class MiniProgram
    {
        /// <summary>
        /// 端点总数（特性声明的路由 + 手工通道路由）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `MiniProgramContractGuards`（MP-X5）。
        /// 现行推导：21 特性路由 + 3 手工通道（小程序码的图片二进制响应通道）= 24；
        /// 按域拆分即 Auth 5 + QrCodeLink 8 + Security 2 + DataAnalysis 9 = 24。
        /// </remarks>
        public const int Endpoints = 24;
    }

    /// <summary>微信支付 APIv3 线（`Tests/Mud.Wechat.Pay.Tests`）。</summary>
    public static class Pay
    {
        /// <summary>
        /// `[HttpClientApi]` 接口数（十个业务域各一）。
        /// </summary>
        /// <remarks>
        /// 锁定于 `WechatPayDomainContractGuards`（PAY-B 系列）。
        /// 现行域集：Transactions / Refund / Bill / Certificates / ProfitSharing / PayScore /
        /// CombineTransactions / Transfer / NewTaxControlFapiao / MarketingFavor。
        /// </remarks>
        public const int HttpApiInterfaces = 10;

        /// <summary>
        /// 端点总数（十域端点之和；账单 / 发票的**文件下载通道**不计入生成式接口）。
        /// </summary>
        /// <remarks>锁定于 `WechatPayDomainContractGuards`。现行推导：原 54 + 直连下单三族（Native / APP / H5）= 57。</remarks>
        public const int Endpoints = 57;
    }
}

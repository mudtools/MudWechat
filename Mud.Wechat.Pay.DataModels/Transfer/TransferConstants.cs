// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transfer;

/// <summary>
/// 商家转账单状态（官方 <c>state</c>，8 值）—— <b>含「是否可原单重试」这一资金安全语义</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716437"/>
/// （2026-10-09 逐值核验，中文说明照录）。
/// </para>
/// <para>
/// <b>🔴 重试语义是本表的重点</b>：<see cref="Accepted"/> 与 <see cref="Processing"/> 两个非终态，
/// 官方明确说明<b>可「原单重试」</b> —— 即<b>不要</b>更换 <c>out_bill_no</c>。
/// 与「发起转账遇错不得换单重试」的红线配套：<b>先查单 → 看本状态 → 原单重试或按终态处理</b>。
/// </para>
/// <para>
/// <b>终态与非终态</b>：终态为 <see cref="Success"/> / <see cref="Fail"/> / <see cref="Cancelled"/>；
/// 其余五个为非终态（须继续查单）。
/// </para>
/// <para>
/// <b>⚠️ 与本域另一套 <c>state</c>（<see cref="TransferElecsignStates"/>）不可混用</b>：
/// 两者都叫 <c>state</c>，但一属转账单、一属电子回单申请单，取值集合完全不同。
/// </para>
/// </remarks>
public static class TransferBillStates
{
    /// <summary>转账已受理（官方 <c>ACCEPTED</c>，非终态）：官方说明「<b>可原单重试</b>」。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>
    /// 转账锁定资金中（官方 <c>PROCESSING</c>，非终态）：官方说明「如果一直停留在该状态，
    /// 建议检查账户余额是否足够，如余额不足，可充值后再<b>原单重试</b>」。
    /// </summary>
    public const string Processing = "PROCESSING";

    /// <summary>待收款用户确认（官方 <c>WAIT_USER_CONFIRM</c>，非终态）：资金已锁定，可拉起微信收款确认页。</summary>
    public const string WaitUserConfirm = "WAIT_USER_CONFIRM";

    /// <summary>转账中（官方 <c>TRANSFERING</c>，非终态）：可拉起微信收款确认页<b>再次重试确认收款</b>。</summary>
    public const string Transfering = "TRANSFERING";

    /// <summary>转账成功（官方 <c>SUCCESS</c>，<b>终态</b>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>
    /// 转账失败（官方 <c>FAIL</c>，<b>终态</b>）：官方说明「若需重新向用户转账，
    /// 请<b>重新生成单据</b>并再次发起」—— 这才是允许换单的时点。
    /// </summary>
    public const string Fail = "FAIL";

    /// <summary>转账撤销中（官方 <c>CANCELING</c>，非终态）：撤销请求已受理，须查单确认最终状态。</summary>
    public const string Canceling = "CANCELING";

    /// <summary>转账撤销完成（官方 <c>CANCELLED</c>，<b>终态</b>）。</summary>
    public const string Cancelled = "CANCELLED";
}

/// <summary>
/// 电子回单申请单状态（官方 <c>state</c>，3 值）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716436"/>
/// （2026-10-09 逐值核验）。注意本状态与<b>转账单</b>状态（<see cref="TransferBillStates"/>）
/// 是<b>两套枚举</b>，只是都叫 <c>state</c> —— 勿混用。
/// </remarks>
public static class TransferElecsignStates
{
    /// <summary>生成中（官方 <c>GENERATING</c>）：已受理成功并在处理中。</summary>
    public const string Generating = "GENERATING";

    /// <summary>已完成（官方 <c>FINISHED</c>）：此时才返回摘要与下载地址。</summary>
    public const string Finished = "FINISHED";

    /// <summary>生成失败（官方 <c>FAILED</c>）：失败原因字段会返回具体原因。</summary>
    public const string Failed = "FAILED";
}

/// <summary>回单文件摘要类型（官方 <c>hash_type</c>）。</summary>
public static class TransferElecsignHashTypes
{
    /// <summary>SHA256 摘要算法（官方 <c>SHA256</c>）。</summary>
    public const string Sha256 = "SHA256";

    /// <summary>国密 SM3 摘要算法（官方 <c>SM3</c>）。</summary>
    public const string Sm3 = "SM3";
}

/// <summary>
/// 用户收款样式类型（官方 <c>user_recv_style.type</c>，<b>2 值</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>
/// （发起转账页的该字段取值表，2026-10-09 逐字核验；更新时间 2025.03.21）。
/// </para>
/// <para>
/// <b>⚠️ 红包样式有硬约束（官方原文）</b>：<see cref="RedPacket"/>「单笔金额需<b>小于等于 200 元</b>，
/// 且<b>仅支持部分转账场景</b>使用」；<see cref="ConfirmPage"/> 无此限制。
/// 官方《产品介绍》的场景表逐场景标注了「是否支持红包样式」
/// （如现金营销支持，而采购货款 / 二手回收 / 公益补助 / 保险理赔<b>不</b>支持）⇒
/// 传 <see cref="RedPacket"/> 前须先核对该场景是否支持，否则会被判参数错。
/// </para>
/// <para>
/// <b>该页<b>未</b>列出默认值</b>：官方只说明两个取值，未言明不传时的默认行为 ⇒
/// 本仓<b>不</b>假定默认值（由官方侧决定）。
/// </para>
/// </remarks>
public static class TransferRecvStyleTypes
{
    /// <summary>收款确认页样式（官方 <c>CONFIRM_PAGE</c>）：用户收款时以标准收款确认页的形式展示。</summary>
    public const string ConfirmPage = "CONFIRM_PAGE";

    /// <summary>红包样式（官方 <c>RED_PACKET</c>）：单笔金额 ≤ 200 元，且仅部分转账场景支持。</summary>
    public const string RedPacket = "RED_PACKET";
}

/// <summary>
/// 商家转账场景 ID（官方 <c>transfer_scene_id</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>⚠️ 官方<b>没有</b>集中式场景 ID 值表（本仓重要留档）</b>：官方《商家转账 - 产品介绍》
/// （<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012711988"/>，更新 2026.08.07）
/// 原文只说「需按实际情况传入转账场景 ID（你可在<b>商户平台 - 产品中心 - 商家转账 - 产品设置</b>
/// 获取及查看场景 ID）」，<b>并未</b>给出取值为 1000 / 1001 / … 的成表清单；
/// 数值由<b>各场景独立页面</b>逐页给出（例如《现金营销》页
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013774588"/> 明确 <c>transfer_scene_id = 1000</c>）。
/// </para>
/// <para>
/// <b>因此本类只收录<b>已由官方页面逐字确证</b>的一项</b>，其余场景<b>不臆造</b>
/// —— 网络上流传的「1000–1011 全场景表」均属二手资料，本仓<b>不</b>采信
/// （场景 ID 还可能与商户实际开通的场景配置相关，官方明确以商户平台展示为准）。
/// </para>
/// <para>
/// <b>使用建议</b>：其余场景请以商户平台「产品设置」中的实际场景 ID 为准传入字符串；
/// 若将来官方发布集中值表，须<b>同批</b>补齐本类与守卫断言。
/// </para>
/// </remarks>
public static class TransferSceneIds
{
    /// <summary>现金营销（官方《现金营销》页逐字确证）：向参与营销活动的用户发放现金奖励。</summary>
    /// <remarks>
    /// 该场景的报备信息须传「活动名称」与「奖励说明」两条（官方原文「有多个字段时需填写完整」）。
    /// </remarks>
    public const string CashMarketing = "1000";
}

/// <summary>
/// 用户收款感知（官方 <c>user_recv_perception</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：官方《商家转账 - 产品介绍》场景表
/// （<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012711988"/>，更新 2026.08.07）
/// 与《现金营销》场景页（更新 2025.07.15）。
/// </para>
/// <para>
/// <b>⚠️ 取值是<b>中文</b>（不是英文枚举）</b>：官方场景表逐字给出 14 个中文取值 ——
/// 这与本仓支付线其它域的英文常量风格完全相反，<b>不得</b>按「枚举都是英文」类推去拼造英文值。
/// </para>
/// <para>
/// <b>🔑 取值与场景强相关（本类最重要的规则）</b>：官方每个场景都有<b>一个默认展示值</b>
/// 与<b>若干需主动传入才可展示的值</b> ——
/// 「转账类型、收款完成页及入账消息标题将根据转账场景<b>自动展示默认内容</b>；
/// 如有其他展示需求，可在发起转账时传入对应的『用户收款感知』」。
/// 例如现金营销：默认 <see cref="ActivityReward"/>，主动传 <see cref="CashReward"/> 才显示为现金奖励；
/// 企业赔付：默认 <see cref="Refund"/>，主动传 <see cref="MerchantCompensation"/>；
/// 佣金报酬：默认 <see cref="LaborRemuneration"/>，主动传 <see cref="Reimbursement"/>；
/// 行政补贴：默认 <see cref="GovernmentSubsidy"/>，主动传 <see cref="GovernmentReward"/>。
/// <b>跨场景乱传值</b>（如给采购货款传「报销款」）属参数错配 ⇒ 消费侧须按场景取值。
/// </para>
/// <para>
/// <b>为何不做「场景 → 可选值」的代码映射</b>：官方该表是<b>文档表格</b>、非接口字段，
/// 且部分行（企业补贴 / 开工利是）未单列场景介绍 ⇒ 用代码固化映射会引入官方未声明的断言。
/// 规则以本 remarks 留档，消费侧按官方页面取值。
/// </para>
/// </remarks>
public static class TransferUserRecvPerceptions
{
    /// <summary>活动奖励（现金营销场景的默认展示值）。</summary>
    public const string ActivityReward = "活动奖励";

    /// <summary>现金奖励（现金营销场景需主动传入才展示）。</summary>
    public const string CashReward = "现金奖励";

    /// <summary>退款（企业赔付场景的默认展示值）。</summary>
    public const string Refund = "退款";

    /// <summary>商家赔付（企业赔付场景需主动传入才展示）。</summary>
    public const string MerchantCompensation = "商家赔付";

    /// <summary>企业补贴（需主动传入才展示）。</summary>
    public const string EnterpriseSubsidy = "企业补贴";

    /// <summary>开工利是（需主动传入才展示）。</summary>
    public const string NewYearBonus = "开工利是";

    /// <summary>劳务报酬（佣金报酬场景的默认展示值）。</summary>
    public const string LaborRemuneration = "劳务报酬";

    /// <summary>报销款（佣金报酬场景需主动传入才展示）。</summary>
    public const string Reimbursement = "报销款";

    /// <summary>货款（采购货款场景的默认展示值）。</summary>
    public const string GoodsPayment = "货款";

    /// <summary>二手回收货款（二手回收场景的默认展示值）。</summary>
    public const string RecyclePayment = "二手回收货款";

    /// <summary>公益补助金（公益补助场景的默认展示值）。</summary>
    public const string CharitySubsidy = "公益补助金";

    /// <summary>行政补贴（行政补贴场景的默认展示值）。</summary>
    public const string GovernmentSubsidy = "行政补贴";

    /// <summary>行政奖励（行政补贴场景需主动传入才展示）。</summary>
    public const string GovernmentReward = "行政奖励";

    /// <summary>保险理赔款（保险理赔场景的默认展示值）。</summary>
    public const string InsuranceClaim = "保险理赔款";
}

/// <summary>
/// 转账场景<b>报备信息类型</b>（官方 <c>transfer_scene_report_infos[].info_type</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：官方《商家转账 - 产品介绍》的「信息类型」表
/// （<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012711988"/>，更新 2026.08.07）
/// 与《现金营销》场景页（更新 2025.07.15）。
/// </para>
/// <para>
/// <b>⚠️ 取值同样是<b>中文</b>，且随场景而定</b>：官方该表按「转账场景 → 信息类型 → 信息内容填写说明」
/// 组织，例如现金营销须传「活动名称」+「奖励说明」，企业赔付须传「赔付原因」，
/// 保险理赔须传「保险产品备案编号」+「保险名称」+「保险操作单号」。
/// 官方原文：「<b>有多个字段时需填写完整</b>」—— 少传会被判参数错。
/// </para>
/// <para>
/// <b>本类只做取值留档、不做「场景 → 必填项」映射</b>（理由同
/// <see cref="TransferUserRecvPerceptions"/>：官方该表是文档表格而非接口契约，
/// 用代码固化会引入官方未声明的断言）。消费侧须按官方页面逐场景填写完整。
/// </para>
/// </remarks>
public static class TransferSceneReportInfoTypes
{
    /// <summary>活动名称（现金营销）。</summary>
    public const string ActivityName = "活动名称";

    /// <summary>奖励说明（现金营销）。</summary>
    public const string RewardDescription = "奖励说明";

    /// <summary>赔付原因（企业赔付）。</summary>
    public const string CompensationReason = "赔付原因";

    /// <summary>岗位类型（佣金报酬）。</summary>
    public const string PositionType = "岗位类型";

    /// <summary>报酬说明（佣金报酬）。</summary>
    public const string RemunerationDescription = "报酬说明";

    /// <summary>采购商品名称（采购货款）。</summary>
    public const string PurchaseGoodsName = "采购商品名称";

    /// <summary>回收商品名称（二手回收）。</summary>
    public const string RecycleGoodsName = "回收商品名称";

    /// <summary>公益活动名称（公益补助）。</summary>
    public const string CharityActivityName = "公益活动名称";

    /// <summary>公益活动备案编号（公益补助）。</summary>
    public const string CharityActivityRecordNo = "公益活动备案编号";

    /// <summary>补贴类型（行政补贴）。</summary>
    public const string SubsidyType = "补贴类型";

    /// <summary>保险产品备案编号（保险理赔）。</summary>
    public const string InsuranceProductRecordNo = "保险产品备案编号";

    /// <summary>保险名称（保险理赔）。</summary>
    public const string InsuranceName = "保险名称";

    /// <summary>保险操作单号（保险理赔）。</summary>
    public const string InsuranceOperationNo = "保险操作单号";
}

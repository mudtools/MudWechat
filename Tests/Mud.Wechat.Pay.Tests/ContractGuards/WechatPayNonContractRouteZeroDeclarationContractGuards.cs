// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 微信支付非契约路由零声明守卫（追踪号 P0-7-PAY-ZERO-DECLARATION-2026-10-14，P1-3 / P1-4 增补 2026-10-15，
/// P1-2-B 增补 2026-10-16，P1-5 增补 2026-10-16，两次取证勘误复核 2026-10-16，
/// 总账收口增补 2026-10-18）：
/// 断言 57 条路由（非契约清单 60 行，同路由多方法各归并 1 条）
/// 在 <c>Src/Pay/</c> 全部源码内<b>零声明</b>，防「网关还能打通」重新诱导加回。
/// <para>
/// <b>判据均为官方文档面</b>：支持商户类型不是本线目标商户、官方文档树内<b>无该端点契约页</b>，
/// 或本仓已有等价现行路由。第三方 SDK 的路由清单本身<b>不是</b>契约。
/// </para>
/// <para>
/// <b>「无该端点契约页」的唯一合格取证口径（本仓两次误判的教训，复核任何一条都须照此）</b>：须在<b>现行官方文档中心</b>
/// （<c>pay.weixin.qq.com/doc/…</c> 树内）按 ①<b>path 关键词</b> ②<b>中文业务名</b> 两路检索并证无
/// （两路皆零命中，且该业务章节树内逐页枚举亦无该路由的契约小节；正文只有产品概述、零 HTTP 契约小节的判不在册）。
/// <b>不得</b>以「第三方 SKIT 清单里的出处 URL（老版 wiki <c>…/chapterX_Y.shtml</c>、旧 <c>docs/…</c> 路径）
/// 301/302/404 落到产品介绍总览页或官方 404 页」为判定依据 —— 旧路径失效与「现行文档中心有无该契约页」
/// <b>是两件事</b>：前者只说明清单出处过期，不说明契约面消失。
/// 社区镜像与第三方 SDK 路由表（wechatpay.js.org / gitbook / SKIT）只能用来<b>找候选页</b>，<b>不得</b>参与「死」判定；
/// 文档中心自带检索口 <c>/doc/search/?keyword=</c> 已失效（302 到 404）、<c>/doc/llms.txt</c> 索引<b>缺章</b>
/// （银行组件 / 爱心餐 / 会员卡 / 消费卡 均已证在册却不在索引内）⇒ 二者<b>不得</b>作「证无」依据。
/// </para>
/// <para>
/// <b>本仓已据此纠正的误判（两次，规模递增）</b>：
/// ① P1-5 的 21 行「判非契约」里 <b>7 行</b>（银行组件六支 + 爱心餐一支）实为现行契约页 ⇒ 已转真缺口落地
/// （<see cref="Mud.Wechat.Pay.IWechatPayCapitalService"/> / <see cref="Mud.Wechat.Pay.IWechatPayLovefeastService"/>，
/// 守卫 CAP / LVF 系列）；
/// ② 余下名册的全量重检（2026-10-16）又证得 <b>24 条误判</b> —— P1-4 旧版会员卡两族<b>整族 22 条</b>
/// （<c>/v3/marketing/membercard-open/*</c> 19 条 + <c>/v3/marketing/membercard-activity/*</c> 3 条，
/// 逐支有现行契约页，如「创建会员卡模板」<c>merchant/4012545584</c> POST <c>/v3/marketing/membercard-open/cards</c>
/// 【普通商户】）与 P1-3 的<b>两支</b>（「发放消费卡」<c>merchant/4017164666</c> POST
/// <c>/v3/marketing/busifavor/coupons/{card_id}/send</c>、「终止合作关系」<c>merchant/4017164460</c> POST
/// <c>/v3/marketing/partnerships/terminate</c>）⇒ 这 24 条<b>已从本名册移出</b>、转为<b>待落地</b>真缺口
/// （批次 P1-6 会员卡两族 / P1-5-H terminate + 消费卡发放）；移出后名册规模由 60 条降为 36 条。
/// 落地同批须给这 24 条补正对照（命中即证「已声明」），<b>不得</b>以「替代关系」为由再并回旧版会员卡
/// （品牌会员卡 <c>/v3/brand/partner/card-member/*</c> 与旧版 <c>membercard-open</c> 是两套并存契约，非新旧替代）。
/// </para>
/// <para>
/// 一、<b>合规证明 Compliance（2 端点）</b>：按 path <c>compliance/certificate</c> 与中文「合规证明」两路检索
/// <b>零命中</b>，现行文档树内无该章节 ⇒ 名册两条属第三方转述漂移路由。
/// <b>勘误</b>：本条原引「官方页面 4012471357 / 4012471359 支持商户为【平台商户】」不实 —— 该两页现行标题是
/// 「发起不活跃商户身份核实 / 查询不活跃商户身份核实结果」，路由为
/// <c>/v3/compliance/inactive-merchant-identity-verification/*</c>（<b>另一模板</b>，同样印【平台商户】⇒ 非本线目标商户）。
/// </para>
/// <para>
/// 二、<b>商户运营 MerchantOperate（3 端点）</b>：按 path <c>merchant-operate/merchant-name-alter</c> 与中文「商家简称」
/// 两路检索，现行文档树内<b>零契约页</b>⇒ 名册三条属第三方转述漂移路由。
/// <b>勘误</b>：本条原引「官方页面支持商户为【平台商户】」的出处不实 —— 商家简称的<b>现行</b>契约页是
/// <c>partner/4016166071</c>「创建商户名称修改申请单」<c>POST /v3/mchalterapply/merchantnamealterapplyment</c>
/// （支持商户【普通服务商】+【平台商户】），本仓<b>已按该路由落位</b>（<see cref="Mud.Wechat.Pay.IWechatPayMerchantAlterApplyService"/>，
/// 守卫 MAA 系列）⇒ 官方侧不存在 <c>/v3/merchant-operate/*</c> 前缀，判据是「路由无官方文档面」而非「支持商户不符」。
/// </para>
/// <para>
/// 三、<b>商户运营管控尽调族 MerchantOperationManage enhanced-due-diligence（3 端点）</b>：现行文档树的
/// 尽调章节（<c>4015629692</c>）<b>恰三支</b>且三支支持商户<b>全部</b>印【平台商户】（非本线目标商户）——
/// 提交 <c>POST …/enhanced-due-diligence/{due_diligence_id}/submit</c>（<c>4015631033</c>）、
/// 上传 <c>POST …/{due_diligence_id}/upload</c>（<c>4015631042</c>）、查询 <c>GET …/{due_diligence_id}</c>（<c>4015631048</c>）。
/// 名册三条的写法（<c>submit</c> 无定位键、以 <c>application_id</c> 定位、<c>cancel</c> 支）与官方路径形状<b>并不相同</b>
/// ⇒ 双重不符（路由无页 + 现行同族三支非目标商户）⇒ 尽调族零端点（守卫 MOM-B2）；本仓已落位的是同前缀下的
/// 管控情况查询支（【普通服务商】，GET <c>/v3/mch-operation-manage/merchant-limitations/sub-mchid/{sub_mchid}</c>），是另一套契约。
/// </para>
/// <para>
/// 四、<b>实名认证 RealName（1 端点）</b>：按 path 检索到的现行页是 <c>partner/4013735179</c>
/// 「查询实名认证结果」<c>GET /v3/realname/verify</c>，支持商户【平台商户】（非本线目标商户）；
/// 名册的 <c>/v3/realname/verify-result</c> 在官方面<b>零命中</b>⇒ 路由无页 + 现行姊妹支非目标商户，双重成立 ⇒ 零端点。
/// <b>勘误</b>：本条原引的 docId 未逐页核对，现已换成上列现行页。
/// </para>
/// <para>
/// 五、<b>商家周周减 MerchantWeeklyDiscount（3 端点）</b>：按 path <c>marketing/favor/platform/media</c> 与中文「周周减」
/// 两路检索，现行文档树内<b>零契约页</b>⇒ 名册三条属转述漂移路由。
/// <b>勘误</b>：本条原引「官方页面『创建 / 激活 / 查询代金券批次』支持商户为【平台商户】」不实 —— 那三页现行正文是
/// 代金券批次族 <c>/v3/marketing/favor/coupon-stocks*</c>，支持商户印【普通服务商】等、<b>无</b>【平台商户】，
/// 且本仓已按该族落位（<see cref="Mud.Wechat.Pay.IWechatPayMarketingFavorService"/>）⇒ 与本条名册路由无关。
/// </para>
/// <para>
/// 六、<b>商家券与营销（P1-3 增补，9 条）</b>：原 13 清单行 / 11 条，其中<b>两支已翻案</b>移出（见条首「误判 ②」：
/// 「发放消费卡」<c>coupons/{card_id}/send</c> 与「终止合作关系」<c>partnerships/terminate</c> 逐页证得在册）。
/// 余下形态两类：
/// ① <b>现行文档树内无该端点契约页</b>（4 条）：补贴凭证三支（<c>subsidy/pay-receipts</c> 两支 +
/// <c>subsidy/return-receipts</c>）—— 现行补贴面是「补贴预下单 / 补贴查询 / 补贴回调」；
/// 代金券按日明细两支（<c>refund-flow-by-day</c> / <c>use-flow-by-day</c>）—— 现行页只有不分日的
/// <c>refund-flow</c> / <c>use-flow</c>（已落位，守卫 MF5、见正对照）。
/// ② <b>路由族整族无页</b>（5 条）：折扣卡三支（<c>/v3/discount-card/*</c>）与提现券一支
/// （<c>/v3/marketing/withdraw-favor/*</c>）按 path 与中文名两路检索皆零命中。
/// <b>形态变更说明</b>：本款原以「清单出处页现行正文是 JSAPI 支付概述页」判定折扣卡 / 提现券五支，
/// 该判据不成立（旧路径失效 ≠ 契约面消失），现改为按 path 在现行文档树检索证无，结论不变。
/// </para>
/// <para>
/// 七、<b>会员卡（旧版）两族 —— 本款整体翻案，已从名册移出</b>：原 P1-4 依「清单 48~75 行的出处全是老版 wiki 路径
/// <c>…/membercard_open/chapterX_Y.shtml</c>、统一 302 落到 <c>merchant/4012062524</c>」判得 28 行 / 22 条路由非契约。
/// <b>该判据无效</b>（见条首合格口径）。按 path 在现行文档中心重检证得两族<b>逐支在册</b>：merchant 树内「会员卡基础功能
/// <c>4012545577</c> / 投放 <c>4012158365</c> / 权益 <c>4012551202</c> / 事件通知 <c>4012552246</c> / 回调通知
/// <c>4012158594</c>」章节下即 <c>/v3/marketing/membercard-open/*</c> 契约页（如「创建会员卡模板」<c>merchant/4012545584</c>
/// POST <c>/v3/marketing/membercard-open/cards</c>【普通商户】），partner 树内另有「产品介绍_会员卡 <c>4012163365</c>」；
/// <c>membercard-activity</c> 三支同判在册 ⇒ 这 22 条<b>不得</b>留在本名册，转为待落地真缺口（批次 P1-6）。
/// 另记两点取证细节：① 品牌会员卡 <c>/v3/brand/partner/card-member/*</c>（本仓已落位 15 端点，
/// 见 <see cref="Mud.Wechat.Pay.IWechatPayBrandCardMemberService"/>）与旧版 <c>membercard-open</c> 是<b>两套并存契约、非新旧替代</b>，
/// 不得以「已有替代品」为由并回；② 「设置会员卡回调」官方动词是 <b>PATCH</b>（<c>4012552251</c>）+ 查询支 GET
/// （<c>4012552275</c>），SKIT 表内记的 POST 不实，落地时以官方页为准。
/// </para>
/// <para>
/// 八、<b>平台服务商解决方案 · 银行转账充值两支（P1-2-B 增补，2 条）</b>：按 path
/// <c>platsolution/ecommerce/bank-transfer-recharges</c> 与中文「银行转账充值」两路检索，现行文档树内<b>零契约页</b>
/// ⇒ 名册两条属转述漂移路由；现行可取证的充值面是 <c>/v3/platsolution/ecommerce/recharges/*</c> 三支
/// （申请 / 按商户单号查询 / 关闭，已落位 <see cref="Mud.Wechat.Pay.IWechatPayPlatsolutionEcommerceRechargeService"/>，
/// 守卫 PEC-RC-B2，见正对照）。
/// <b>勘误</b>：本款原以「出处页 <c>partner/4012763053</c> / <c>4012763057</c> 双双 404」判定，该判据不合格
/// （旧路径失效 ≠ 契约面消失），现改为按 path 检索证无 —— 结论维持，依据已换。
/// </para>
/// <para>
/// 九、<b>行业方案三支（P1-5 增补，3 条）</b>：本款三条的判定<b>复核后维持</b>，依据改为合格口径：
/// ① <c>POST /papay/pay/transactions/apply</c>（清单行 85）—— 其引用页 <c>merchant/4012467036</c> 现行正文是
/// <c>POST /papay/pay/schedules/contract-id/{contract_id}/schedule</c>（张冠李戴），且按 path 检索，现行 PAPay 的
/// <c>…/transactions/apply</c> 只有 <c>insurance-pay</c> 与 <c>pay/harmony</c> 两支（已落位，见正对照），裸支零命中；
/// ② <c>POST /payroll-card/wesure/token-validations</c>（清单行 104）—— 引用页 <c>partner/4012461785</c> 现行正文是
/// <c>GET /payroll-card/authentications/{authenticate_number}</c>，且 <c>/payroll-card/wesure/*</c> 族按 path 检索零契约页；
/// ③ <c>POST /payroll-card/wesure/insurance</c>（清单行 103）—— 同属 <c>/payroll-card/wesure/*</c> 族零命中
/// （原引的出处是老版 wiki <c>chapter4_1_32.shtml</c>，落点为产品总览页，<b>不</b>作判据）。
/// </para>
/// <para>
/// 十、<b>行业方案余十支（P1-5-F 增补，10 条）</b>：本条是<b>按上述合格口径逐条重检</b>的产物 ——
/// P1-5 原判非契约 21 行中，七行经重检证为现行契约页（已转真缺口，见条首勘误说明与正对照），余下这些支
/// 在现行文档中心两路检索<b>确证零契约页</b>，形态三类：
/// ① <b>路由族整族无页</b>（6 条）：共享充电宝三支（<c>/hire-power-bank/insurance-orders</c>、
/// <c>…/{out_order_no}</c>、<c>/hire-power-bank/user-qualifications/{openid}</c>）—— 现行充电宝章节
/// <c>merchant/4012587281</c> 是产品概述，其接口链接只指<b>支付分</b>三支（<c>/v3/payscore/serviceorder</c> 族，
/// 本仓已落位）；刷脸硬件两支（<c>/iotmanage/devices</c>、<c>/iotmanage/device-inspections/{device_sn}</c>）
/// 按 path <c>iotmanage</c> 与中文「设备巡检」两路检索<b>零契约页</b>（旧刷脸 wiki 里的 v0.0.1 设备管理面
/// 未迁入现行文档中心，<b>不得</b>以「旧 wiki 未迁移」为判据）；<c>/express/users/{openid}/transform</c> 属「微信快递服务」，
/// 现行运单 API 在<b>小程序线</b> <c>/cgi-bin/express/business/*</c>（已落位），<c>/express/*</c> 前缀零命中。
/// ② <b>表内零官方出处</b>（2 条）：<c>/pay-devices/printers/{device_id}/print-orders</c> 与其查询支 —— SKIT 表内
/// 该两行<b>没有任何</b>出处 URL，且 <c>/pay-devices/*</c> 在现行文档树与全部已取证件正文零命中；
/// 点金计划「商家小票管理」是另一路由族（<c>/v3/goldplan/merchants/*</c>，已落位，见正对照）。
/// ③ <b>旧 v2 能力被官方明令停用</b>（2 条）：<c>/papay/contracts/{contract_id}/fail-notify</c> 与
/// <c>/partner-papay/contracts/{contract_id}/fail-notify</c> 按 path <c>fail-notify</c> 检索<b>零契约页</b>，
/// 其出处只是 <c>doc/v2</c> 根（<b>不作</b>判据）；判定的文档面依据是 APIv3 委托代扣现行页
/// 原文「若此前有调用…<b>请立即停止使用</b>」⇒ 官方已停用的 v2 面不构成 v3 契约，现行失败通知面是
/// <c>…/contracts/{contract_id}/notify</c> 两支（已落位，见正对照）。
/// 另记一行<b>非路由条目</b>：原清单第 50 行的 <c>/{download_url}</c> 是电子发票「获取发票下载信息」
/// （<c>merchant/4012538335</c>）<b>应答里的文件下载地址字段</b>，不是商户侧可声明的端点 ⇒ 不入本名册，
/// 由 <c>NewTaxControlFapiao</c> 域守卫（FPX 系列）按字段建模。
/// </para>
/// <para>
/// 十一、<b>总账收口增补（2026-10-18，21 条 / 24 清单行）</b>：本轮对 §8 残余名册逐条重检，形态三类：
/// ① <b>平台商户专属面</b>（15 条）—— 提现申请三联（<c>/v3/mch_operate/risk/withdrawl-apply</c>，
/// 官方页 <c>4012488950</c> / <c>4012778400</c> / <c>4012476164</c> 均印【平台商户】）、
/// 不活跃商户身份核实两支（<c>/v3/compliance/inactive-merchant-identity-verification/*</c>）、
/// 实名认证 <c>GET /v3/realname/verify</c>（<c>partner/4013735179</c> 印【平台商户】）、
/// 平台商户提现族三支（<c>/v3/merchant/fund/withdraw/*</c>，与 C1 裁决 / MF-B2 同源）、
/// 尽调族官方现行两支（<c>{due_diligence_id}</c> 定位，<c>4015631033</c> / <c>4015631042</c> 印【平台商户】）、
/// 合单支付平台商户面两支（<c>4012602242</c> 查单 / <c>4012602225</c> 小程序下单，均印【平台商户】，
/// 与已落位的普通服务商面 <c>/jsapi</c> 是<b>两套契约</b>）—— 本仓目标商户不含【平台商户】⇒ 零端点；
/// ② <b>官方现行只到 APIv2</b>（2 条）—— 服务商付款码 <c>codepay</c> 与 <c>reverse</c> 两页
/// （<c>partner/4012382179</c> / <c>4012382181</c>）正文明确「付款码支付目前仍为V2接口」⇒ 不构成 v3 契约
/// （v2 面由 APIv2 线承载，与本名册无关）；
/// ③ <b>现行文档树无契约页</b>（4 条）—— 电商分账售后单两支（EC-R7 已裁）、
/// 商家周周减三支（旧 wiki <c>…/Offline/…</c> 落产品介绍页，非契约页）、
/// 收单曝光 <c>/v3/pay/acquiring-bank/transactions/exposure</c>（出处页 <c>4014326541</c> 现行 302→404，
/// 且该「订单曝光」仅供内部使用）—— 均零端点。
/// 商户侧 <c>/v3/pay/transactions/exposure</c> 与分期曝光三支已按现行页落位，见正对照。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 本守卫为「清单即契约」型断言：57 条路由（60 清单行）的增减都必须先按条首的<b>合格取证口径</b>复核
/// 现行官方文档中心（path + 中文名两路检索 + 章节树逐页枚举），不得因网关可达、第三方 SDK 在册
/// 或「旧 wiki / 旧 docs 路径已失效」而放宽或收紧。
/// </para>
/// </remarks>
public class WechatPayNonContractRouteZeroDeclarationContractGuards
{
    /// <summary>
    /// 非契约路由清单（57 条 = Compliance 2 + MerchantOperate 3 + 尽调族 3 + RealName 1 + 周周减 3
    /// + P1-3 商家券与营销 9 + P1-2-B 银行转账充值 2 + P1-5 行业方案 3 + P1-5-F 行业方案余十支 10
    /// + 总账收口增补 21；
    /// P1-3 的 11 清单行里「补贴凭证 pay-receipts 的 POST 与 GET」同路由 ⇒ 2 行折成 1 条，
    /// 余下各款一律一行一条。
    /// <b>已整体移出的两批</b>：P1-4 会员卡（旧版）两族 28 行 / 22 条 + P1-3 的「发放消费卡」「终止合作关系」
    /// 2 行 / 2 条 —— 按现行文档中心逐页重检证得<b>在册</b>（详见类注释条首「误判 ②」与第七条），
    /// 转为待落地真缺口（批次 P1-6 / P1-5-H）；<b>不得</b>因「第三方 SKIT 清单的旧 wiki 出处已失效」再并回本名册。
    /// 路由计数按 (方法, 路由) 的去重口径同样为 0，因为名册内这些支都不存在）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 合规证明 Compliance（2 端点）：现行文档树按 path 与中文名两路检索零契约页。
        "/v3/compliance/certificate/download",
        "/v3/compliance/certificate/query",

        // 商户运营 MerchantOperate（3 端点）：/v3/merchant-operate/* 前缀零契约页；
        // 商家简称的现行契约面是 /v3/mchalterapply/merchantnamealterapplyment 族（已落位，守卫 MAA）。
        "/v3/merchant-operate/merchant-name-alter/apply",
        "/v3/merchant-operate/merchant-name-alter/application/{application_id}",
        "/v3/merchant-operate/merchant-name-alter/application/{application_id}/cancel",

        // 商户运营管控尽调族（3 端点）：现行尽调章节恰三支、定位键是 due_diligence_id 且全印【平台商户】，
        // 名册三条的 path 形状在官方面零命中（既无页又非目标商户，见类注释第三款）。
        "/v3/mch-operation-manage/enhanced-due-diligence/submit",
        "/v3/mch-operation-manage/enhanced-due-diligence/{application_id}",
        "/v3/mch-operation-manage/enhanced-due-diligence/{application_id}/cancel",

        // 实名认证 RealName（1 端点）：现行页是 GET /v3/realname/verify【平台商户】，名册路由零命中。
        "/v3/realname/verify-result",

        // 商家周周减 MerchantWeeklyDiscount（3 端点）：/v3/marketing/favor/platform/media-* 零契约页
        // （原引的三页实为 coupon-stocks 族【普通服务商】，本仓已落位，见类注释第五款勘误）。
        "/v3/marketing/favor/platform/media-create",
        "/v3/marketing/favor/platform/media-start",
        "/v3/marketing/favor/platform/media-query/{stock_id}",

        // 商家券补贴凭证三支（3 端点）：官方补贴面现行页只有「补贴预下单 / 补贴查询 / 补贴回调」，
        // pay-receipts 与 return-receipts 在文档树内无契约页。
        "/v3/marketing/busifavor/subsidy/pay-receipts",
        "/v3/marketing/busifavor/subsidy/pay-receipts/{subsidy_receipt_id}",
        "/v3/marketing/busifavor/subsidy/return-receipts",

        // 代金券按日明细两支（2 端点）：现行页只有不分日的 refund-flow / use-flow（已落位，守卫 MF5）。
        "/v3/marketing/favor/stocks/{stock_id}/refund-flow-by-day",
        "/v3/marketing/favor/stocks/{stock_id}/use-flow-by-day",

        // 折扣卡三支（3 端点）：/v3/discount-card/* 族按 path 与中文名两路检索零契约页。
        "/v3/discount-card/cards",
        "/v3/discount-card/cards/{out_card_code}",
        "/v3/discount-card/cards/{out_card_code}/add-user-records",

        // 提现券（1 条 / 2 清单行：查询与发放同路由）：/v3/marketing/withdraw-favor/* 族零契约页。
        "/v3/marketing/withdraw-favor/users/{openid}/coupons",

        // 平台服务商解决方案 · 银行转账充值两支（P1-2-B 第八条）：按 path 与中文名两路检索零契约页；
        // 现行可取证面是 /v3/platsolution/ecommerce/recharges/* 三支（已落位，见下方正对照）。
        "/v3/platsolution/ecommerce/bank-transfer-recharges/apply",
        "/v3/platsolution/ecommerce/bank-transfer-recharges/out-recharge-no/{out_recharge_no}",

        // 行业方案三支（P1-5 第九条）：PAPay 裸 apply 支的出处页实为 schedule 支（张冠李戴），
        // 现行 apply 面只有 insurance-pay 与 pay/harmony 两支；wesure 两支按 path 检索零契约页
        // （token-validations 引的页实为认证单单查支）。
        "/v3/papay/pay/transactions/apply",
        "/v3/payroll-card/wesure/insurance",
        "/v3/payroll-card/wesure/token-validations",

        // 微信快递服务转单（P1-5-F 第十条 ①）：/express/* 前缀在现行文档树零命中；现行运单 API 属小程序线
        // /cgi-bin/express/business/*（已落位，非本线路由 ⇒ 本条只锁支付线不出现该族）。
        "/v3/express/users/{openid}/transform",

        // 共享充电宝三支（P1-5-F 第十条 ①）：现行充电宝章节 merchant/4012587281 是产品概述，
        // 其接口链接只指支付分三支（/v3/payscore/serviceorder 族，已落位）⇒ hire-power-bank 路由族零契约页。
        "/v3/hire-power-bank/insurance-orders",
        "/v3/hire-power-bank/insurance-orders/{out_order_no}",
        "/v3/hire-power-bank/user-qualifications/{openid}",

        // 刷脸硬件设备管理两支（P1-5-F 第十条 ①）：按 path iotmanage 与中文「设备巡检」两路检索零契约页
        // （旧 wxfacepay wiki 的 v0.0.1 设备管理面未迁入现行文档中心，该事实只作背景、不作判据）。
        "/v3/iotmanage/devices",
        "/v3/iotmanage/device-inspections/{device_sn}",

        // 服务商小票打印两支（P1-5-F 第十条 ②）：/pay-devices/* 在现行文档树与全部已取证件正文零命中
        // （SKIT 表内这两行本身也没有任何官方出处）；点金计划「商家小票管理」/v3/goldplan/merchants/* 是另一路由族（已落位，见正对照）。
        "/v3/pay-devices/printers/{device_id}/print-orders",
        "/v3/pay-devices/printers/{device_id}/print-orders/{print_order_no}",

        // 委托代扣扣费失败通知两支（P1-5-F 第十条 ③）：按 path fail-notify 检索零契约页，
        // 且 APIv3 现行页原文「若此前有调用…请立即停止使用」⇒ 官方已停用的 v2 面不构成 v3 契约，现行失败通知面是
        // …/contracts/{contract_id}/notify 两支（已落位，见正对照）。
        "/v3/papay/contracts/{contract_id}/fail-notify",
        "/v3/partner-papay/contracts/{contract_id}/fail-notify",

        // —— 总账收口增补（2026-10-18，21 条 / 24 清单行，见类注释第十一款）——

        // 平台商户提现族三支：官方页 4012476670 / 4012476674 / 4012476678 / 4012476672 全印【平台商户】
        // （与 C1 裁决 / MF-B2 同源；本仓余额面只落 4012721477 / 4012721446 两支【普通服务商】）。
        "/v3/merchant/fund/withdraw/{withdraw_id}",
        "/v3/merchant/fund/withdraw/bill-type/{bill_type}",
        "/v3/merchant/fund/withdraw/out-request-no/{out_request_no}",

        // 电商分账售后单（POST/GET 同路由，EC-R7）：官方 after-sales-orders 目录内无该端点正文 ⇒ 两行归一。
        "/v3/ecommerce/profitsharing/after-sales-orders",

        // 服务商付款码与撤销（2 条）：官方页正文明确「付款码支付目前仍为V2接口」⇒ v3 面无契约，
        // v2 面由 APIv2 线承载；不得照 SKIT 的 v3 形状落地。
        "/v3/pay/partner/transactions/codepay",
        "/v3/pay/partner/transactions/out-trade-no/{out_trade_no}/reverse",

        // 合单支付平台商户面两支：4012602242 查单 / 4012602225 小程序下单均印【平台商户】，
        // 与已落位的普通服务商面（/jsapi 等）是两套契约 ⇒ 不混入 IWechatPayCombineService。
        "/v3/combine-transactions/id/{combine_transaction_id}",
        "/v3/combine-transactions/miniprogram",

        // 服务商支付分「商户收款」（POST/GET 两清单行同路由）：官方无此端点，
        // PartnerPayScore 接口 remarks 已裁定（partner 面只有创单/查单/取消/完结/改单/同步/消息提醒）。
        "/v3/payscore/partner/serviceorder/{out_order_no}/pay",

        // 不活跃商户身份核实两支：官方页 4012471357 / 4012471359 印【平台商户】⇒ 零端点。
        "/v3/compliance/inactive-merchant-identity-verification/merchants",
        "/v3/compliance/inactive-merchant-identity-verification/merchants/{sub_mchid}/verifications/{verification_id}",

        // 实名认证查询：现行页 partner/4013735179 印【平台商户】⇒ 零端点（名册的 verify-result 已在上方）。
        "/v3/realname/verify",

        // 提现申请单三支：4012488950（提交）/ 4012778400（按申请单号查）/ 4012476164（按商户单号查）
        // 均印【平台商户】⇒ 零端点。
        "/v3/mch_operate/risk/withdrawl-apply",
        "/v3/mch_operate/risk/withdrawl-apply/applyment-id/{applyment_id}",
        "/v3/mch_operate/risk/withdrawl-apply/out-request-no/{out_request_no}",

        // 商户运营管控尽调官方现行两支：4015631033（提交）/ 4015631042（上传）印【平台商户】
        // （与上方名册里的三条「另形」路由同族不同形，均零端点）。
        "/v3/mch-operation-manage/enhanced-due-diligence/{due_diligence_id}/submit",
        "/v3/mch-operation-manage/enhanced-due-diligence/{due_diligence_id}/upload",

        // 商家周周减三支：SKIT 出处为旧 wiki …/Offline/… （落产品介绍页，非契约页），
        // 按 path 与中文名两路检索现行文档树零契约页（与上方 favor/platform/media 三支同族裁决）。
        "/v3/marketing/weekly-discount/activities",
        "/v3/marketing/weekly-discount/activities/{activity_id}",
        "/v3/marketing/weekly-discount/activities/{activity_id}/apply",

        // 收单曝光：出处页 4014326541 现行 302→404，且「订单曝光」仅供内部使用；
        // 现行可落位的是商户侧 /v3/pay/transactions/exposure 与分期两支（见正对照）。
        "/v3/pay/acquiring-bank/transactions/exposure",
    };

    /// <summary>扫描根：支付线 Src/Pay/ 下全部 .cs 文件。</summary>
    private static readonly DirectoryInfo PaySrcRoot = new(
        Path.GetFullPath(Path.Combine(typeof(WechatPayNonContractRouteZeroDeclarationContractGuards).Assembly.Location, "..", "..", "..", "..", "..", "..", "Src", "Pay")));

    /// <summary>读取 Src/Pay/ 全源码（排除 obj/bin）—— 两个断言共用同一扫描口径。</summary>
    private static string ReadPaySource()
    {
        var csFiles = PaySrcRoot.GetFiles("*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.FullName.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                           && !f.FullName.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(csFiles);

        return string.Join("\n", csFiles.Select(static f => File.ReadAllText(f.FullName)));
    }

    /// <summary>
    /// 路由 → 匹配式：官方清单里的 path 占位符是 snake_case（<c>{stock_id}</c>），而源码声明的是
    /// C# 参数名（<c>{stockId}</c>）⇒ 占位符段一律收成通配，否则「改名落位」会绕过本守卫变成假绿。
    /// </summary>
    private static string RoutePattern(string route) =>
        "\"" + Regex.Replace(route, @"\{[^}]*\}", "[^/\"]*") + "\"";

    /// <summary>零声明主断言：清单内每条非契约路由在 <c>Src/Pay/</c> 全源码里以「引号字面量」形态零命中。</summary>
    [Fact]
    public void NonContractRoutes_ShouldHaveZeroDeclarationsInPaySource()
    {
        var allSource = ReadPaySource();

        // 名册规模（57 条）与文档注释同批：清单行增减须先按条首合格口径复核现行官方文档中心，不能只改注释不改数组。
        NonContractRoutes.Should().HaveCount(57, "非契约路由名册规模须与类注释一致（60 清单行按同路由归并）");
        NonContractRoutes.Distinct().Should().HaveCount(57, "同一路径不得在名册里出现两次（归并口径由注释逐族写明）");

        foreach (var route in NonContractRoutes)
        {
            Regex.Matches(allSource, RoutePattern(route)).Should().BeEmpty(
                $"非契约路由 {route} 在 Src/Pay/ 出现声明：判据是现行官方文档中心两路检索（path + 中文业务名），不得因网关可达、第三方 SDK 在册或旧 wiki 路径失效而加回");
        }
    }

    /// <summary>正对照：已落位路由必须被同一扫描口径命中，证明上面的零命中是真裁决而不是扫描失效。</summary>
    [Fact]
    public void PositiveControl_AlreadyDeclaredRoutes_ShouldBeFound()
    {
        // 正对照：已落位的路由必须能在源码中找到，确保扫描本身有效。
        // 覆盖「纯字面量 / 单个占位符 / 占位符在段中」三种形状，其中后若干条与零声明清单成对
        // （favor stocks 族对照 refund-flow-by-day，busifavor users 族对照 withdraw-favor 两支，
        // 品牌会员卡 cards 两支对照旧版 membercard-open cards 族 ⇒ 两族<b>并存</b>，若把旧版并回名册即红），
        // partnerships 一支对照已翻案移出的 terminate（同前缀在册 ⇒ 名册里不得再出现 terminate），
        // 末段七条专用于 P1-5-G 勘误（判非契约误用的七行，命中即证明它们是真契约而非零声明）。
        var positiveRoutes = new[]
        {
            // Transactions 域
            "/v3/pay/transactions/jsapi",
            // MerchantStore 域
            "/v3/merchant-store/stores",
            // MerchantService 域
            "/v3/merchant-service/complaints-v2",
            // MarketingFavor 域：零声明的 refund-flow-by-day 的现行姊妹支（占位符名 = C# 参数名）
            "/v3/marketing/favor/stocks/{stockId}/refund-flow",
            // MarketingBusifavor 域：openid 占位符在 path（与零声明的 withdraw-favor 两支同形状）
            "/v3/marketing/busifavor/users/{openid}/coupons",
            // MarketingPartnerships 域：在册的建立 / 查询两支（已翻案的 partnerships/terminate 同前缀
            // ⇒ 本条命中即证明「terminate 属真缺口、不是零声明」的判定口径可被扫描面复核）
            "/v3/marketing/partnerships",
            // BrandCardMember 域：品牌会员卡 cards 两支。与旧版 /v3/marketing/membercard-open/cards 族
            // （已翻案、待落地 P1-6）是<b>两套并存契约</b>，不是替代关系；同尾段 cards / cards/{card_id} 形状
            "/v3/brand/partner/card-member/cards",
            "/v3/brand/partner/card-member/cards/{cardId}",
            // PlatsolutionEcommerceRecharge 域：零声明的 bank-transfer-recharges 两支的现行替代面
            // （同为「申请 + 按商户单号查询」两动词形状，占位符名 = C# 参数名 ⇒ 对照匹配式同样有效）
            "/v3/platsolution/ecommerce/recharges/apply",
            "/v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}",
            // Papay / PayrollCard 域：零声明的裸 apply 支与 wesure 两支的现行同族面
            // （同为 `/papay/pay/*transactions/apply` 形状、同处 `/payroll-card/` 前缀内 ⇒ 扫描根若漂移即红）
            "/v3/papay/pay/harmony/transactions/apply",
            "/v3/payroll-card/tokens",
            "/v3/payroll-card/authentications/{authenticateNumber}",
            // Papay / PartnerPapay 域：零声明的 fail-notify 两支的现行替代面（只差末段 notify↔fail-notify，
            // 占位符名 = C# 参数名 ⇒ 匹配式同族；若把零声明条误写成 notify 即被本对照打回）。
            "/v3/papay/contracts/{contractId}/notify",
            "/v3/partner-papay/contracts/{contractId}/notify",
            // GoldPlan 域：零声明的 /pay-devices/* 打印两支在现行树里的同业务替代面（商家小票管理，
            // 同为「多段字面量、零占位符」形状 ⇒ 字面量匹配式失效即红）。
            "/v3/goldplan/merchants/close-advertising-show",
            // Multiuse 域：与零声明的 /pay-devices/printers/{device_id}/print-orders/{print_order_no}
            // 同形状（两占位符 + 动作段）的已落位支。
            "/v3/multiuse/stocks/{stockId}/send-flow",
            // P1-5-G 勘误反钉（关键）：原 P1-5 判非契约的 21 行里有 7 行是<b>现行契约页</b>（旧 wiki 301 落点
            // 不构成判据），已转真缺口落地 ⇒ 这七条路由「必须被扫描命中」。
            // 一旦有人把它们塞回零声明名册（或按旧判据重新判非契约），本对照即红。
            "/v3/capital/capitallhh/areas/provinces",
            "/v3/capital/capitallhh/areas/provinces/{provinceCode}/cities",
            "/v3/capital/capitallhh/banks/corporate-banking",
            "/v3/capital/capitallhh/banks/personal-banking",
            "/v3/capital/capitallhh/banks/{bankAliasCode}/branches",
            "/v3/capital/capitallhh/banks/search-banks-by-bank-account",
            "/v3/lovefeast/brands/{brandId}",
        };

        var allSource = ReadPaySource();

        foreach (var route in positiveRoutes)
        {
            Regex.Matches(allSource, RoutePattern(route)).Should().NotBeEmpty(
                $"正对照路由 {route} 未被扫描口径命中：扫描根或匹配式已漂移，零声明断言将静默恒真");
        }
    }
}

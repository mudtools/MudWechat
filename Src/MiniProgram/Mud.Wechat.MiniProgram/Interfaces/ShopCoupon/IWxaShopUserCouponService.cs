// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ShopCoupon;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 用户优惠券」域接口（4 端点：添加用户券 / 更新用户券 /
/// 用户券列表 / 用户券状态更新；官方 <c>minishopopencomponent2/API/coupon</c> 分区的用户券族）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 与小券线 <c>/channels/ec/*</c> 令牌链无关（守卫 SC7 锁定两线不交叉）。
/// </para>
/// <para>
/// <b>本域是「商家侧记账」而非「平台侧发券」</b>：用户券记录由商家在自有领券流程里<b>回写</b>给微信
/// （官方 <c>receive_info.limit_num_one_person</c> / <c>total_num</c> 明写「只做展示，领券回调时接入方判断有无超领」）
/// ⇒ 本域四支端点写入的是商家侧的领取 / 核销台账，微信侧不会替商家生成这些记录。
/// </para>
/// <para>
/// <b>主键是复合定位</b>：<c>openid</c> + <c>out_user_coupon_id</c>（商家自拟、须唯一）+ <c>out_coupon_id</c>（券本体主键）
/// 三支共同定位一条用户券记录；<c>out_user_coupon_id</c> 与券侧 <c>out_coupon_id</c> 同为<b>商家侧 id</b>，
/// 微信不返回任何服务端 id ⇒ 重复添加同 <c>out_user_coupon_id</c> 属覆盖写、非幂等安全，不得放进自动重试管道。
/// </para>
/// <para>
/// <b>写侧包装 vs 读侧扁平（本域最大的形态陷阱）</b>：<see cref="AddUserCouponAsync"/> 与
/// <see cref="UpdateUserCouponAsync"/> 的请求体是 <c>{openid, user_coupon:{…}, recv_time}</c>（<b>有 <c>user_coupon</c> 包装</b>），
/// 而 <see cref="GetUserCouponListAsync"/> 的 <c>result_list[]</c> 元素是<b>平铺</b>对象（<c>openid</c> 与业务字段同级、
/// <b>无</b> <c>user_coupon</c> 包装）⇒ 两套类型不互抄（守卫 SC5 锁定）。
/// </para>
/// <para>
/// <b>状态值域与券本体不同套</b>：用户券状态为 100 生效中 / 101 已过期 / 102 已经使用 / 103 已主动废弃，
/// 与 <see cref="IWxaShopCouponService"/> 的 1~4 是<b>两套枚举</b>，官方两页各给一张枚举-status 表 ⇒ 不得混用、不得合并建模。
/// </para>
/// <para>
/// <b>敏感面</b>：<c>openid</c> 是用户在该小程序下的唯一标识（个人敏感标识），
/// <c>ext_info.use_time</c> 间接暴露用户核销行为 ⇒ 一律<b>不得写入日志、遥测或异常消息</b>，
/// 不得拼进 <c>ToString()</c> 或异常 Message；请求体 DTO 亦不得被宿主整体打日志。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本域 4 页官方正文均<b>未给</b>端点级频率数字、权限集编号、云调用方法名与第三方代调用说明
/// （2026-10-18 逐页取证）⇒ 注释留档该空缺；「<c>page_size</c> 最大 200」是规模限制，与频率分列。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopCoupon", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopUserCouponService
{
    /// <summary>
    /// 添加用户优惠券（<c>POST /shop/coupon/add_user_coupon</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/add_usercoupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> + <c>user_coupon</c> 包装 + <c>recv_time</c>），见 <see cref="WxaShopUserCouponAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方回包示例只有 <c>errcode</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>user_coupon</c> 包装内三支键（<c>out_user_coupon_id</c> /
    /// <c>out_coupon_id</c> / <c>status</c>）与顶层 <c>openid</c> 共四支<b>全部必填</b>，
    /// <c>recv_time</c> 为<b>选填</b>（官方原文「领取时间（秒级时间戳）」）。
    /// </para>
    /// <para>
    /// <b>路由与文档文件名不同形</b>：接口地址是 <c>add_user_coupon</c>（带下划线），文档页名却是
    /// <c>add_usercoupon.html</c>（连写）⇒ 守卫 SC1 按<b>官方接口地址</b>锁定路由，勿按文件名拼。
    /// </para>
    /// <para><b>状态取值</b>：100 生效中 / 101 已过期 / 102 已经使用 / 103 已主动废弃；新建通常置 100。</para>
    /// <para><b><c>openid</c> 禁落日志</b>（用户级敏感标识）；<c>out_coupon_id</c> 必须是本商家已添加过的券，否则记录无法关联。</para>
    /// </remarks>
    [Post("/shop/coupon/add_user_coupon")]
    Task<WxaResponse> AddUserCouponAsync(
        [Body] WxaShopUserCouponAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新用户优惠券（<c>POST /shop/coupon/update_user_coupon</c>，核销时间由本端点写入）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/update_usercoupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> + <c>user_coupon</c> 包装 + <c>recv_time</c>），见 <see cref="WxaShopUserCouponUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；本页参数表与添加侧<b>有两处不可互换的差异</b>：
    /// ① <c>user_coupon.status</c> <b>不在本页参数表内</b>（改状态请走 <see cref="UpdateUserCouponStatusAsync"/>）；
    /// ② <c>recv_time</c> 在本页是<b>必填</b>（添加侧为选填）⇒ 两个请求体类型不得合并（守卫 SC5 锁定）。
    /// </para>
    /// <para>
    /// <b>核销落账口径</b>：<c>user_coupon.ext_info.use_time</c>（用户优惠券核销时间，秒级时间戳）官方注明「未核销不填」
    /// ⇒ 与 <see cref="UpdateUserCouponStatusAsync"/> 置 <c>102</c>（已经使用）是<b>两个动作</b>：
    /// 只写 <c>use_time</c> 不改状态、或只改状态不留核销时间，都会造成台账不一致，宿主须成对调用。
    /// </para>
    /// <para><b>覆盖式写入</b>：<c>user_coupon</c> 内的 <c>ext_info</c> 为整份替换（本页 <c>ext_info</c> 只有 <c>use_time</c> 一支键）。</para>
    /// <para><b><c>openid</c> 与 <c>use_time</c> 禁落日志、遥测与异常消息</b>（用户级敏感标识与行为数据）。</para>
    /// </remarks>
    [Post("/shop/coupon/update_user_coupon")]
    Task<WxaResponse> UpdateUserCouponAsync(
        [Body] WxaShopUserCouponUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户优惠券列表（<c>POST /shop/coupon/get_usercoupon_list</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/get_usercoupon_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page_size</c> 必填、<c>offset</c> 选填、<c>openid</c> 见 remarks），见 <see cref="WxaShopUserCouponGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>total_num</c> + <c>result_list[]</c>（<b>平铺</b>条目），见 <see cref="WxaShopUserCouponGetListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；翻页口径为<b>偏移式</b>：<c>page_size</c> 必填（官方原文「最大为 200」）、
    /// <c>offset</c> 选填（官方原文「从 0 开始」）。
    /// </para>
    /// <para>
    /// <b>官方本页参数表与请求示例不一致（诚实留档）</b>：请求示例携带 <c>openid</c>，而「请求参数说明」表只列
    /// <c>page_size</c> / <c>offset</c> 两行 ⇒ 本仓按示例把 <c>openid</c> 建模为<b>可空</b>字段（用于按用户过滤），
    /// 宿主若官方侧不认该键则回落为全量列表，不得依赖它做强约束。
    /// </para>
    /// <para>
    /// <b>条目是平铺结构</b>：<c>result_list[]</c> 元素直接把 <c>openid</c> 与业务字段放在同一层
    /// （<c>{out_user_coupon_id, openid, out_coupon_id, status, create_time, update_time, start_time, end_time, ext_info}</c>），
    /// <b>没有</b>写侧的 <c>user_coupon</c> 包装 ⇒ 与 <see cref="WxaShopUserCoupon"/> 不共用类型。
    /// </para>
    /// <para>
    /// <b>官方示例里 <c>start_time</c> / <c>end_time</c> 会出现 <c>0</c></b>（未填有效期时）⇒ 不得当作合法时间戳参与区间判断；
    /// <c>ext_info</c> 亦可能为空对象（未核销）。
    /// </para>
    /// <para><b>本端点应答不含券本体信息</b>（券名、折扣、有效期规则都要另取 <see cref="IWxaShopCouponService"/>）。</para>
    /// </remarks>
    [Post("/shop/coupon/get_usercoupon_list")]
    Task<WxaShopUserCouponGetListResponse> GetUserCouponListAsync(
        [Body] WxaShopUserCouponGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新用户优惠券状态（<c>POST /shop/coupon/update_usercoupon_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/update_usercoupon_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> + <c>out_coupon_id</c> + <c>out_user_coupon_id</c> + <c>status</c> 四支），见 <see cref="WxaShopUserCouponStatusUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{openid, out_coupon_id, out_user_coupon_id, status}</c>（<b>平铺、无包装</b>，
    /// 与本域写侧另两支的 <c>user_coupon</c> 包装形态不同）；
    /// <b>本页官方没有「请求参数说明」表</b>（只有请求示例与枚举-status 表）⇒ 字段集取自请求示例，已在此留档。
    /// </para>
    /// <para>
    /// <b>路由拼写照官方原文 <c>update_usercoupon_status</c></b>（<c>usercoupon</c> 连写）：本仓三条用户券写端点的路由
    /// 分别是 <c>add_user_coupon</c>（下划线）/ <c>update_user_coupon</c>（下划线）/ <c>update_usercoupon_status</c>（连写），
    /// <b>三种形态混用即官方事实</b>，不得「顺手对齐」成一致拼写（守卫 SC1 / SC5 双锁）。
    /// </para>
    /// <para><b>状态取值</b>：100 生效中 / 101 已过期 / 102 已经使用 / 103 已主动废弃；官方本页未给状态流转限制，但置 102 时须按 <see cref="UpdateUserCouponAsync"/> 补记 <c>use_time</c>。</para>
    /// <para><b><c>openid</c> 禁落日志、遥测与异常消息</b>；四支键缺一即无法定位记录（官方示例四支齐给）。</para>
    /// </remarks>
    [Post("/shop/coupon/update_usercoupon_status")]
    Task<WxaResponse> UpdateUserCouponStatusAsync(
        [Body] WxaShopUserCouponStatusUpdateRequest request,
        CancellationToken cancellationToken = default);
}

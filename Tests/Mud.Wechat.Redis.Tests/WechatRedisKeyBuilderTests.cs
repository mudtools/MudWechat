// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R1：统一键构造器护栏（R-01 / R-20 / R2-02 / RD2 / glob 转义）。
/// </summary>
public class WechatRedisKeyBuilderTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Combine_ShouldThrowInvalidOperationException_WhenPrefixIsEmpty(string? prefix)
    {
        var act = () => WechatRedisKeyBuilder.Combine(prefix!, "token");
        act.Should().Throw<InvalidOperationException>().WithMessage("*R-01*");
    }

    [Fact]
    public void Combine_ShouldThrowInvalidOperationException_WhenPrefixStartsWithWildcard()
    {
        var act = () => WechatRedisKeyBuilder.Combine("*wechat", "token");
        act.Should().Throw<InvalidOperationException>().WithMessage("*R-01*");
    }

    [Fact]
    public void Combine_ShouldEscapeColonInsideSegment_WhenSegmentContainsColon()
    {
        WechatRedisKeyBuilder.Combine("wechat", "corpa", "a:b", "corp1")
            .Should().Be(@"wechat:corpa:a\:b:corp1");
    }

    [Fact]
    public void Combine_ShouldSkipEmptySegments_WhenSegmentIsNullOrEmpty()
    {
        WechatRedisKeyBuilder.Combine("wechat", "", null, "corp1")
            .Should().Be("wechat:corp1");
    }

    [Fact]
    public void Combine_ShouldThrowInvalidArgumentFailure_WhenSegmentExceedsLengthLimit()
    {
        var longSegment = new string('x', 257);
        var act = () => WechatRedisKeyBuilder.Combine("wechat", longSegment);
        act.Should().Throw<WechatRedisException>()
            .Which.FailureKind.Should().Be(WechatRedisFailureKind.InvalidArgument);
    }

    [Fact]
    public void Pattern_ShouldEndWithColonStar_AndShareSourceWithCombine()
    {
        var pattern = WechatRedisKeyBuilder.Pattern("wechat", "corpa", "a");
        var key = WechatRedisKeyBuilder.Combine("wechat", "corpa", "a", "corp1");

        pattern.Should().EndWith(":*");
        // 段级精确匹配：模式去掉 ":*" 后恰为键的父前缀。
        key.Should().StartWith(pattern[..^1]);
    }

    [Fact]
    public void Pattern_ShouldEscapeGlobMetacharacters_WhenPrefixContainsThem()
    {
        // KeyPrefix 含 glob 元字符（校验器只拦 '*' 前缀）时，模式必须字面量匹配而非失控扩张（§6.2 护栏 4）。
        var pattern = WechatRedisKeyBuilder.Pattern("wechat?x", "corpa");
        pattern.Should().Be(@"wechat\?x:corpa:*");
    }

    [Fact]
    public void Pattern_ShouldDoubleEscapeBackslash_WhenSegmentEscapesColon()
    {
        // R2-02/D10 同源：Combine 的 "\:" 转义在 glob 侧须再转义为 "\\:"，否则 SCAN 永不命中。
        var pattern = WechatRedisKeyBuilder.Pattern("wechat", "corpa", "a:b");
        pattern.Should().Be(@"wechat:corpa:a\\:b:*");
    }

    [Fact]
    public void Pattern_ShouldMatchOwnKeys_ForArbitrarySegmentCombinations()
    {
        // 属性式：任意段组合下模式必命中自产键（含通配符字符段）——模式段 + 叶子段构成完整键。
        var segmentsList = new[]
        {
            new[] { "corpa", "a" },
            new[] { "corpa", "a*b" },
            new[] { "corpa", "a:b" },
            new[] { "corpa", "corp[1]" },
            new[] { "corpa", "a?b" },
            new[] { "corpa", @"c\orp" },
        };

        foreach (var segments in segmentsList)
        {
            var key = WechatRedisKeyBuilder.Combine("wechat", segments[0], segments[1], "leaf");
            var pattern = WechatRedisKeyBuilder.Pattern("wechat", segments);

            GlobMatch(pattern, key).Should().BeTrue($"pattern={pattern} 应命中自产键 key={key}");
        }
    }

    [Fact]
    public void Pattern_ShouldNotMatchSiblingKey_WhenSegmentIsPrefixOfAnother()
    {
        var pattern = WechatRedisKeyBuilder.Pattern("wechat", "corpa", "a");
        var sibling = WechatRedisKeyBuilder.Combine("wechat", "corpa", "ab", "corp1");

        GlobMatch(pattern, sibling).Should().BeFalse("段级精确匹配防前缀兄弟键越界（R2-02）");
    }

    [Fact]
    public void TokenKey_ShouldKeepStoreKeyVerbatim_WhenStoreKeyContainsColons()
    {
        // RD2：storeKey 为不透明叶子，三段 ':' 原样保留。
        WechatRedisKeyBuilder.TokenKey("wechat", "Wechat.AccessToken:default:default")
            .Should().Be("wechat:token:Wechat.AccessToken:default:default");
    }

    [Theory]
    [InlineData("Wechat.AccessToken:default:default")]
    [InlineData("Wechat.AccessToken:app:corp1:extra")]
    [InlineData("Wechat.SuiteAccessToken:suite:")]
    [InlineData("")]
    public void TryStripTokenKeyPrefix_ShouldRoundTripStoreKey_WhenKeyIsTokenDomainKey(string storeKey)
    {
        var redisKey = WechatRedisKeyBuilder.TokenKey("wechat", storeKey);

        var stripped = WechatRedisKeyBuilder.TryStripTokenKeyPrefix(redisKey, "wechat", out var restored);
        stripped.Should().BeTrue();
        restored.Should().Be(storeKey);
    }

    [Fact]
    public void TryStripTokenKeyPrefix_ShouldReturnFalse_WhenKeyBelongsToOtherDomain()
    {
        WechatRedisKeyBuilder.TryStripTokenKeyPrefix("wechat:corpa:a:corp1", "wechat", out _)
            .Should().BeFalse();
    }

    [Fact]
    public void TryStripTokenKeyPrefix_ShouldReturnFalse_WhenPrefixDiffers()
    {
        var redisKey = WechatRedisKeyBuilder.TokenKey("wechat", "Wechat.AccessToken:default:default");
        WechatRedisKeyBuilder.TryStripTokenKeyPrefix(redisKey, "wechat-dev", out _)
            .Should().BeFalse("多环境前缀隔离不得互相剥前缀");
    }

    /// <summary>测试内最小 stringmatchlen 语义实现（\ 转义 + * ? [ ] 元字符），服务属性式断言。</summary>
    private static bool GlobMatch(string pattern, string input)
    {
        var regex = new StringBuilder("^");
        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            if (c == '\\' && i + 1 < pattern.Length)
            {
                regex.Append(Regex.Escape(pattern[++i].ToString()));
            }
            else if (c == '*')
            {
                regex.Append(".*");
            }
            else if (c == '?')
            {
                regex.Append('.');
            }
            else
            {
                regex.Append(Regex.Escape(c.ToString()));
            }
        }

        return Regex.IsMatch(input, regex + "$");
    }
}

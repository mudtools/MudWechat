// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R4/T-R2 支撑面：配置校验与连接装配（Validate / ToString 脱敏 / NormalizeKeyPrefix / TLS 推导）。
/// </summary>
public class WechatRedisOptionsTests
{
    [Fact]
    public void Validate_ShouldPass_WhenDefaultOptions()
    {
        var act = () => new WechatRedisOptions().Validate();
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("redis.example.com")]
    public void Validate_ShouldThrow_WhenServerAddressIsInvalid(string? serverAddress)
    {
        var options = new WechatRedisOptions();
        options.Connection.ServerAddress = serverAddress!;

        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*ServerAddress*");
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public void Validate_ShouldThrow_WhenConnectTimeoutBelowFloor(int connectTimeout)
    {
        var options = new WechatRedisOptions { Connection = { ConnectTimeout = connectTimeout } };
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*ConnectTimeout*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenSyncTimeoutBelowFloor()
    {
        var options = new WechatRedisOptions { Connection = { SyncTimeout = 500 } };
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*SyncTimeout*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenConnectRetryNegative()
    {
        var options = new WechatRedisOptions { Connection = { ConnectRetry = -1 } };
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*ConnectRetry*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldThrow_WhenKeyPrefixEmpty(string keyPrefix)
    {
        var options = new WechatRedisOptions { KeyPrefix = keyPrefix };
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*KeyPrefix*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenKeyPrefixStartsWithWildcard()
    {
        var options = new WechatRedisOptions { KeyPrefix = "*wechat" };
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*KeyPrefix*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenSuiteTicketTtlNegative()
    {
        var options = new WechatRedisOptions { SuiteTicketTtl = TimeSpan.FromSeconds(-1) };
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>().WithMessage("*SuiteTicketTtl*");
    }

    [Fact]
    public void ToString_ShouldMaskPassword()
    {
        var options = new WechatRedisOptions { Connection = { ServerAddress = "localhost:6379", Password = "super-secret" } };
        var text = options.ToString();

        text.Should().Contain("***");
        text.Should().NotContain("super-secret");
    }

    [Theory]
    [InlineData(null, "wechat")]
    [InlineData("", "wechat")]
    [InlineData("   ", "wechat")]
    [InlineData("wechat-dev", "wechat-dev")]
    public void NormalizeKeyPrefix_ShouldFallBackToDefault_WhenValueIsBlank(string? raw, string expected)
    {
        WechatRedisOptions.NormalizeKeyPrefix(raw).Should().Be(expected);
    }
}

/// <summary>
/// T-R2 支撑面：连接选项装配（显式字段映射 + TLS 推导 + ClientName 兜底）。
/// </summary>
public class RedisConnectionFactoryTests
{
    [Fact]
    public void Build_ShouldThrowArgumentNullException_WhenOptionsNull()
    {
        var act = () => RedisConnectionFactory.Build(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Build_ShouldMapConnectionFields_WhenOptionsGiven()
    {
        var options = new WechatRedisOptions
        {
            Connection =
            {
                ServerAddress = "redis.example.com:6380",
                Password = "pass",
                DefaultDatabase = 3,
                ConnectTimeout = 6000,
                SyncTimeout = 7000,
                ConnectRetry = 5,
                AbortOnConnectFail = false,
            },
            Advanced = { AllowAdmin = true, ClientName = "my-client" },
        };

        var config = RedisConnectionFactory.Build(options);

        config.EndPoints.Should().ContainSingle();
        config.Password.Should().Be("pass");
        config.DefaultDatabase.Should().Be(3);
        config.ConnectTimeout.Should().Be(6000);
        config.SyncTimeout.Should().Be(7000);
        config.ConnectRetry.Should().Be(5);
        config.AbortOnConnectFail.Should().BeFalse();
        config.AllowAdmin.Should().BeTrue();
        config.ClientName.Should().Be("my-client");
    }

    [Fact]
    public void Build_ShouldRequireSsl_WhenServerAddressUsesRedissScheme()
    {
        // R2-26：Parse 不因 rediss:// scheme 自动置 Ssl，必须显式推导。
        var config = RedisConnectionFactory.Build(new WechatRedisOptions
        {
            Connection = { ServerAddress = "rediss://redis.example.com:6380" },
        });

        config.Ssl.Should().BeTrue();
    }

    [Fact]
    public void Build_ShouldNotRequireSsl_WhenPlainHostPort()
    {
        var config = RedisConnectionFactory.Build(new WechatRedisOptions());
        config.Ssl.Should().BeFalse();
    }

    [Fact]
    public void Build_ShouldFallbackClientName_WhenNotConfigured()
    {
        var config = RedisConnectionFactory.Build(new WechatRedisOptions());
        config.ClientName.Should().StartWith("Wechat-Redis-");
    }

    [Theory]
    [InlineData("rediss://host:6380", true)]
    [InlineData("REDISS://host:6380", true)]
    [InlineData("redis://host:6379", false)]
    [InlineData("host:6379", false)]
    [InlineData("", false)]
    public void RequiresSsl_ShouldDetectRedissSchemeCaseInsensitively(string serverAddress, bool expected)
    {
        RedisConnectionFactory.RequiresSsl(serverAddress).Should().Be(expected);
    }
}

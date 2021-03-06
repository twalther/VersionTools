﻿using System;
using System.Linq;
using Xunit;

namespace VersionTools.Lib.Test {
    public class SemverTest {
        public class Parse_method {
            [Fact]
            public void should_parse_a_normal_version_without_prerelease_and_build_info() {
                var semver = Semver.Parse("1.2.3");

                Assert.Equal(1, semver.Major);
                Assert.Equal(2, semver.Minor);
                Assert.Equal(3, semver.Patch);
            }

            [Fact]
            public void should_parse_a_version_with_prerelese_information() {
                var semver = Semver.Parse("1.2.3-beta.2");

                Assert.Equal("beta.2", semver.PreRelease);
            }

            [Fact]
            public void should_parse_a_version_with_build_meta_data_but_no_prerelase_info() {
                var semver = Semver.Parse("1.2.3+master.c34fede");

                Assert.Equal("master.c34fede", semver.Build);
            }

            [Fact]
            public void should_parse_a_version_with_both_prerelease_and_build_info() {
                var semver = Semver.Parse("1.2.3-beta+master");

                Assert.Equal(1,        semver.Major);
                Assert.Equal(2,        semver.Minor);
                Assert.Equal(3,        semver.Patch);
                Assert.Equal("beta",   semver.PreRelease);
                Assert.Equal("master", semver.Build);
            }

            [Fact]
            public void should_throw_a_format_exception_when_the_input_string_is_an_invalid_semver() {
                Assert.Throws<FormatException>(() => Semver.Parse("1.2-foo"));
            }
        }

        public class IsValidSemver_method {
            [Fact]
            public void should_return_true_when_arg_is_a_valid_semver_version() {
                Assert.True( Semver.IsValidSemver("1.2.3-pre+build"));
            }


            [Fact]
            public void should_return_false_when_args_is_not_a_valid_semver_version() {
                Assert.False( Semver.IsValidSemver("foobar"));
            }

        }

        public class OverrideBuild_method {
            [Fact]
            public void should_create_a_new_version_with_the_specified_build() {
                var version = "1.2.3-pre+build";
                var semver = Semver.Parse(version);

                var newSemver = semver.OverrideBuild("new.build");
                Assert.Equal("1.2.3-pre+new.build", newSemver.FullVersion);
            }

        }

        public class FullVersion_property {
            [Fact]
            public void should_display_the_full_version_including_prerelease_and_build_info() {
                var version = "1.2.3-pre+build";
                var semver = Semver.Parse(version);

                Assert.Equal(version, semver.FullVersion);
            }
        }

        public class Version_property {
            [Fact]
            public void should_display_the_string_value_of_the_normal_version() {
                var version = "1.2.3-pre+build";
                var semver = Semver.Parse(version);

                Assert.Equal("1.2.3", semver.Version);
            }
        }

        public class ToString_method {
            [Fact]
            public void should_display_the_full_version_when_called_without_argument() {
                var semver = new Semver(1,2,3,"pre", "build");

                Assert.Equal("1.2.3-pre+build", semver.ToString());
            }

            [Fact]
            public void should_display_prerelease_version_when_called_with_P_argument() {
                var semver = new Semver(1,2,3,"pre", "build");
                
                Assert.Equal("1.2.3-pre", semver.ToString("P"));
            }

            [Fact]
            public void should_display_full_version_when_called_with_F_argument() {
                var semver = new Semver(1,2,3,"pre", "build");

                Assert.Equal("1.2.3-pre+build", semver.ToString("F"));
            }

            [Fact]
            public void should_display_normal_version_when_called_with_V_argument() {
                var semver = new Semver(1,2,3,"pre", "build");

                Assert.Equal("1.2.3", semver.ToString("V"));
            }

            [Fact]
            public void should_throw_format_exception_when_format_is_not_a_valid_value() {
                var semver = new Semver(1,2,3,"pre", "build");

                Assert.Throws<FormatException>(() => semver.ToString("X"));
            }

            [Fact]
            public void should_display_normal_version_when_called_with_P_argument_and_version_is_not_prerelease() {
                var semver = new Semver(1,2,3,"", "build");
                
                Assert.Equal("1.2.3", semver.ToString("P"));
            }
        }

        public class IsPreRelease_property {
            [Fact]
            public void should_return_true_when_version_is_a_pre_release() {
                var semver = new Semver(1,2,3,"pre");
                
                Assert.True( semver.IsPreRelease );
            }


            [Fact]
            public void should_return_false_when_version_is_not_a_pre_release() {
                var semver = new Semver(1, 2, 3);

                Assert.False( semver.IsPreRelease );
            }

        }
        public class CompareTo_method {
            [Fact]
            public void should_return_0_when_versions_are_equal() {
                var v1 = Semver.Parse("1.2.3");
                var v2 = Semver.Parse("1.2.3");

                var compared = v1.CompareTo(v2);

                Assert.Equal(0, compared);
            }

            [Fact]
            public void should_not_include_build_in_comparison() {
                var v1 = Semver.Parse("1.2.3+fuu");
                var v2 = Semver.Parse("1.2.3+bar");

                var compared = v1.CompareTo(v2);

                Assert.Equal(0, compared);
            }

            [Fact]
            public void should_return_minus_1_when_args_patch_nr_is_greater_than_own_patch_nr_other_things_equal() {
                var v1 = Semver.Parse("1.2.1");
                var v2 = Semver.Parse("1.2.2");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }

            [Fact]
            public void should_return_minus_1_when_args_minor_nr_is_greater_than_own_minor_nr_other_things_equal() {
                var v1 = Semver.Parse("1.0.0");
                var v2 = Semver.Parse("1.1.0");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }

            [Fact]
            public void should_return_minus_1_when_args_major_nr_is_greater_than_own_major_nr_other_things_equal() {
                var v1 = Semver.Parse("2.1.0");
                var v2 = Semver.Parse("3.1.0");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }
            [Fact]
            public void should_return_1_when_args_patch_nr_is_less_than_own_patch_nr_other_things_equal() {
                var v1 = Semver.Parse("1.2.3");
                var v2 = Semver.Parse("1.2.2");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }

            [Fact]
            public void should_return_1_when_args_minor_nr_is_less_than_own_minor_nr_other_things_equal() {
                var v1 = Semver.Parse("1.2.0");
                var v2 = Semver.Parse("1.1.0");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }

            [Fact]
            public void should_return_1_when_args_major_nr_is_less_than_own_major_nr_other_things_equal() {
                var v1 = Semver.Parse("2.1.0");
                var v2 = Semver.Parse("1.1.0");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }


            [Fact]
            public void should_return_1_when_arg_is_prerelease() {
                var v1 = Semver.Parse("1.2.3");
                var v2 = Semver.Parse("1.2.3-pre");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }


            [Fact]
            public void should_return_minus_1_when_self_is_prerelease() {
                var v1 = Semver.Parse("1.2.3-pre");
                var v2 = Semver.Parse("1.2.3");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }


            [Fact]
            public void should_return_1_when_own_prerelease_has_more_fields_than_other() {
                var v1 = Semver.Parse("1.2.3-pre.1");
                var v2 = Semver.Parse("1.2.3-pre");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }

            [Fact]
            public void should_return_minus_1_when_own_prerelease_has_less_fields_than_other() {
                var v1 = Semver.Parse("1.2.3-pre");
                var v2 = Semver.Parse("1.2.3-pre.1");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }


            [Fact]
            public void should_return_1_when_own_numeric_prerelease_is_greater_than_other() {
                var v1 = Semver.Parse("1.2.3-2");
                var v2 = Semver.Parse("1.2.3-1");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }

            [Fact]
            public void should_return_minus_1_when_own_numeric_prerelease_is_less_than_other() {
                var v1 = Semver.Parse("1.2.3-1");
                var v2 = Semver.Parse("1.2.3-2");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }


            [Fact]
            public void should_return_1_when_own_prerelease_is_alphanum_and_other_is_numeric() {
                var v1 = Semver.Parse("1.2.3-alpha");
                var v2 = Semver.Parse("1.2.3-1");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1, compared);
            }

            [Fact]
            public void should_return_minus_1_when_own_prerelease_is_numeric_and_other_is_alphanum() {
                var v1 = Semver.Parse("1.2.3-1");
                var v2 = Semver.Parse("1.2.3-alpha");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1, compared);
            }


            [Fact]
            public void should_return_1_when_own_prerelease_is_lexically_greater_than_other() {
                var v1 = Semver.Parse("1.2.3-beta");
                var v2 = Semver.Parse("1.2.3-alpha");

                var compared = v1.CompareTo(v2);

                Assert.Equal(1,compared);
            }


            [Fact]
            public void should_return_minus_1_when_own_prerelease_is_lexically_less_than_other() {
                var v1 = Semver.Parse("1.2.3-alpha");
                var v2 = Semver.Parse("1.2.3-beta");

                var compared = v1.CompareTo(v2);

                Assert.Equal(-1,compared);
            }


            [Fact]
            public void should_ignore_case_when_comparing_prereleases() {
                var v1 = Semver.Parse("1.2.3-alpha");
                var v2 = Semver.Parse("1.2.3-ALPHA");

                var compared = v1.CompareTo(v2);

                Assert.Equal(0, compared);
            }


            [Fact]
            public void should_handle_the_example_precedence_chain_on_the_semver_webpage() {
                var versions = new[] {
                    "1.0.0-alpha",
                    "1.0.0-alpha.1",
                    "1.0.0-alpha.beta",
                    "1.0.0-beta",
                    "1.0.0-beta.2",
                    "1.0.0-beta.11",
                    "1.0.0-rc.1",
                    "1.0.0"
                }
                .Select(Semver.Parse)
                .ToArray();

                for (var i = 1; i < versions.Length; i++) {
                    var comparedToPrevious = versions[i].CompareTo(versions[i - 1]);
                    Assert.Equal(1, comparedToPrevious);
                }
            }
        }

        public class Equals_method {
            [Fact]
            public void should_return_true_when_versions_are_lexically_equal() {
                var v1 = Semver.Parse("1.2.3-beta");
                var v2 = Semver.Parse("1.2.3-beta");

                var areEqual = v1.Equals(v2);
                Assert.True(areEqual);
            }


            [Fact]
            public void should_ignore_build_info() {
                var v1 = Semver.Parse("1.2.3-beta+foo");
                var v2 = Semver.Parse("1.2.3-beta");

                var areEqual = v1.Equals(v2);
                Assert.True(areEqual);
            }


            [Fact]
            public void should_ignore_case() {
                var v1 = Semver.Parse("1.2.3-beta");
                var v2 = Semver.Parse("1.2.3-BETA");

                var areEqual = v1.Equals(v2);
                Assert.True(areEqual);
            }


            [Fact]
            public void should_return_false_when_versions_are_not_equal() {
                var v1 = Semver.Parse("1.2.4-beta");
                var v2 = Semver.Parse("1.2.3-beta");

                var areEqual = v1.Equals(v2);
                Assert.False(areEqual);
            }


            [Fact]
            public void should_return_false_when_other_object_is_null() {
                var v1 = Semver.Parse("1.2.4-beta");
                
                Assert.False( v1.Equals(null));
            }

            [Fact]
            public void should_return_false_when_other_object_is_not_a_semver() {
                var v1 = Semver.Parse("1.2.4-beta");
                
                Assert.False( v1.Equals(new object()));
            }
 
        }

        public class GetHashCode_method {
            [Fact]
            public void should_return_the_same_code_for_equal_versions() {
                var code1 = Semver.Parse("1.2.3-beta");
                var code2 = Semver.Parse("1.2.3-beta");

                Assert.Equal( code1.GetHashCode(), code2.GetHashCode());
            }


            [Fact]
            public void should_return_different_codes_for_different_versions() {
                var code1 = Semver.Parse("1.3.3-beta");
                var code2 = Semver.Parse("1.2.3-beta");

                Assert.NotEqual( code1.GetHashCode(), code2.GetHashCode());
            }


            [Fact]
            public void should_ignore_build_meta_data() {
                var code1 = Semver.Parse("1.2.3-beta+build1");
                var code2 = Semver.Parse("1.2.3-beta+build2");

                Assert.Equal( code1.GetHashCode(), code2.GetHashCode());
            }

        }

        public class operator_equals {
            [Fact]
            public void should_return_true_when_versions_are_equal() {
                var ver1 = Semver.Parse("1.2.3-beta");
                var ver2 = Semver.Parse("1.2.3-beta");

                Assert.True( ver1 == ver2 );
            }

            [Fact]
            public void should_return_false_when_versions_are_not_equal() {
                var ver1 = Semver.Parse("1.2.3-beta");
                var ver2 = Semver.Parse("1.2.3-charlie");

                Assert.False( ver1 == ver2 );
            }
        }
        
        public class operator_not_equals {
            [Fact]
            public void should_return_false_when_versions_are_equal() {
                var ver1 = Semver.Parse("1.2.3-beta");
                var ver2 = Semver.Parse("1.2.3-beta");

                Assert.False( ver1 != ver2 );
            }

            [Fact]
            public void should_return_true_when_version_are_not_equal() {
                var ver1 = Semver.Parse("1.2.3-beta");
                var ver2 = Semver.Parse("1.2.3-charlie");

                Assert.True( ver1 != ver2 );
            }
        }
        
        public class operator_greater_than {
            [Fact]
            public void should_return_true_when_left_operand_is_greater_than_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("1.9.9");

                Assert.True( ver1 > ver2 );
            }

            [Fact]
            public void should_return_false_when_left_operand_is_less_than_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("2.9.9");

                Assert.False( ver1 > ver2 );
            }

            [Fact]
            public void should_return_false_when_left_operand_is_equal_to_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("2.0.0");

                Assert.False( ver1 > ver2 );
            }
        }

        public class operator_less_than {
            [Fact]
            public void should_return_true_when_left_operand_is_less_than_right_operand() {
                var ver1 = Semver.Parse("1.0.0");
                var ver2 = Semver.Parse("1.9.9");

                Assert.True( ver1 < ver2 );
            }

            [Fact]
            public void should_return_false_when_left_operand_is_greater_than_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("1.9.9");

                Assert.False( ver1 < ver2 );
            }

            [Fact]
            public void should_return_false_when_left_operand_is_equal_to_right_operand() {
                var ver1 = Semver.Parse("2.0.0-beta");
                var ver2 = Semver.Parse("2.0.0-beta");

                Assert.False( ver1 < ver2 );
            }
        }

        public class operator_less_than_or_equal {
            [Fact]
            public void should_return_true_when_left_operand_is_less_than_right_operand() {
                var ver1 = Semver.Parse("1.0.0");
                var ver2 = Semver.Parse("1.9.9");

                Assert.True( ver1 <= ver2 );
            }

            [Fact]
            public void should_return_false_when_left_operand_is_greater_than_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("1.9.9");

                Assert.False( ver1 <= ver2 );
            }

            [Fact]
            public void should_return_true_when_left_operand_is_equal_to_right_operand() {
                var ver1 = Semver.Parse("2.0.0-beta");
                var ver2 = Semver.Parse("2.0.0-beta");

                Assert.True( ver1 <= ver2 );
            }
        }

        public class operator_greater_than_or_equal {
            [Fact]
            public void should_return_true_when_left_operand_is_greater_than_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("1.9.9");

                Assert.True( ver1 >= ver2 );
            }

            [Fact]
            public void should_return_false_when_left_operand_is_less_than_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("2.9.9");

                Assert.False( ver1 >= ver2 );
            }

            [Fact]
            public void should_return_true_when_left_operand_is_equal_to_right_operand() {
                var ver1 = Semver.Parse("2.0.0");
                var ver2 = Semver.Parse("2.0.0");

                Assert.True( ver1 >= ver2 );
            }
        }
    }

}
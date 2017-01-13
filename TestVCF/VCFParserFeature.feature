Feature: VCFParserFeature
	In order to take advantage of information stored in VCF file
	As a user
	I want to read variant elements

@mytag
Scenario Outline: Parse a VCF file
	Given A VCF file, <filePath>
	When I open the file
	Then count the number of variants in the file

Examples: 
| filePath                              |
| 13_328_A73P06X9Q-A73806XBP.merged.vcf |
| sample1.vcf                           |
| sample2.vcf                           |
| GHDX29.vcf                            |
| GHDX31.vcf                            |
| GHDX32.vcf                            |
| GHDX33.vcf                            |
| GHDX35.vcf                            |
| GHDX36.vcf                            |
| GHDX37.vcf                            |
| GHDX38.vcf                            |
| GHDX39.vcf                            |
| GHDX40.vcf                            |
| GHDX41.vcf                            |
| empty.vcf								|
| general.vcf							|


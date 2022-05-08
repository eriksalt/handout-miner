using Azure.Search.Documents.Indexes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
{
    internal class SkillsetManager
    {
        AzureConfig _config;
        public SkillsetManager(AzureConfig config)
        {
            _config = config;   
        }

        public SearchIndexerSkillset GetSkillset()
        {
            //2Do: Clean up hyphens- http://handoutminerskills.azurewebsites.net
            //2Do: Parse metadata and add metadata into text field
            //2Do: Annotation of images
            //2Do: video processing

            return new SearchIndexerSkillset(
                name: _config.skillset_name,
                skills: new List<SearchIndexerSkill>()
                {
                    new OcrSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "image")
                            {
                                Source = "/document/normalized_images/*"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "text"),
                            new OutputFieldMappingEntry(name: "layoutText")
                        })
                    {
                        Context = "/document/normalized_images/*",
                        DefaultLanguageCode = OcrSkillLanguage.En,
                        ShouldDetectOrientation = true
                    },
                    new WebApiSkill(inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "sourceText")
                            {
                                Source = "/document/normalized_images/*/text"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "cleanText")
                        },
                        uri: string.Format("{0}/api/remove-hyphenation?code={1}", _config.custom_skills_site, _config.custom_skills_key))
                    {
                        Description = "remove hyphens from ocr text",
                        Context = "/document/normalized_images/*",
                        BatchSize = 1
                    },
                    new ImageAnalysisSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "image")
                            {
                                Source = "/document/normalized_images/*"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "tags")
                            {
                                TargetName = "Tags"
                            },
                            new OutputFieldMappingEntry(name: "description")
                            {
                                TargetName = "Description"
                            }
                        })
                    {
                        Context = "/document/normalized_images/*",
                        VisualFeatures = { VisualFeature.Tags, VisualFeature.Description },
                        Details = { ImageDetail.Celebrities, ImageDetail.Landmarks },
                        DefaultLanguageCode = ImageAnalysisSkillLanguage.En
                    },
                    new MergeSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/content"
                            },
                            new InputFieldMappingEntry(name: "itemsToInsert")
                            {
                                Source = "/document/normalized_images/*/cleanText"
                            },
                            new InputFieldMappingEntry(name: "offsets")
                            {
                                Source = "/document/normalized_images/*/contentOffset"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "mergedText")
                            {
                                TargetName = "nativeTextAndOcr"
                            }
                        })
                    {
                        Description = "Merge native text content and inline OCR content where images were present",
                        Context = "/document"
                    },
                    new MergeSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/nativeTextAndOcr"
                            },
                            new InputFieldMappingEntry(name: "itemsToInsert")
                            {
                                Source = "/document/normalized_images/*/Description/captions/*/text"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "mergedText")
                            {
                                TargetName = "fullTextAndCaptions"
                            }
                        })
                    {
                        Description = "Merge text content with image captions",
                        Context = "/document"
                    },
                    new MergeSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/fullTextAndCaptions"
                            },
                            new InputFieldMappingEntry(name: "itemsToInsert")
                            {
                                Source = "/document/normalized_images/*/Tags/*/name"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "mergedText")
                            {
                                TargetName = "finalText"
                            }
                        })
                    {
                        Description = "Merge text content with image tags",
                        Context = "/document"
                    },
                    new SplitSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/finalText"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "textItems")
                            {
                                TargetName = "pages"
                            }
                        })
                    {
                        Description = "Split text into pages for subsequent skill processing",
                        Context = "/document/finalText",
                        TextSplitMode = TextSplitMode.Pages,
                        MaximumPageLength = 5000

                    },
                    new LanguageDetectionSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/finalText"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "languageCode")
                        }),
                    new EntityRecognitionSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/finalText/pages/*"
                            },
                            new InputFieldMappingEntry(name: "languageCode")
                            {
                                Source = "/document/languageCode"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "persons")
                            {
                                TargetName = "people"
                            },
                            new OutputFieldMappingEntry(name: "locations"),
                            new OutputFieldMappingEntry(name: "organizations"),
                            new OutputFieldMappingEntry(name: "dateTimes"),
                            new OutputFieldMappingEntry(name: "namedEntities"),
                            new OutputFieldMappingEntry(name: "entities")
                        })
                    {
                        Context = "/document/finalText/pages/*",
                        Categories = { EntityCategory.Person, EntityCategory.Location, EntityCategory.Datetime, EntityCategory.Organization },
                    }
                })
            {
                Name = _config.skillset_name,
                Description = "Handout Miner Skillset",
                CognitiveServicesAccount = new CognitiveServicesAccountKey(key: _config.cog_srv_key) 
            };
        }
    }
}

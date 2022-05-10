﻿using Azure.Search.Documents.Indexes.Models;
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
            //2Do GPS data
            //2Do: Annotation of images
            //2Do: Facetable?
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
                            new OutputFieldMappingEntry(name: "resultText"){TargetName ="cleanText"}
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
                            new OutputFieldMappingEntry(name: "description")
                            {
                                TargetName = "Description"
                            },
                            new OutputFieldMappingEntry(name: "tags")
                            {
                                TargetName = "Tags"
                            }
                        })
                    {
                        Context = "/document/normalized_images/*",
                        VisualFeatures = { VisualFeature.Description, VisualFeature.Tags },
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
                                Source = "/document/normalized_images/*/Tags/*/name"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "mergedText")
                            {
                                TargetName = "fullTextAndTags"
                            }
                        })
                    {
                        Description = "Merge text content with image tags",
                        Context = "/document"
                    },
                    new MergeSkill(
                        inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "text")
                            {
                                Source = "/document/fullTextAndTags"
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
                                TargetName = "fullTextTagsAndDescription"
                            }
                        })
                    {
                        Description = "Merge text content with descripton",
                        Context = "/document"
                    },
                    new WebApiSkill(inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "firstText")
                            {
                                Source = "/document/fullTextTagsAndDescription"
                            },
                            new InputFieldMappingEntry(name: "secondText")
                            {
                                Source = "/document/blobdescription"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "resultText"){TargetName ="finalText"}
                        },
                        uri: string.Format("{0}/api/concatenate?code={1}", _config.custom_skills_site, _config.custom_skills_key))
                    {
                        Description = "merge metadata tags from blob metadata into text",
                        Context = "/document",
                        BatchSize = 1
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
                            new OutputFieldMappingEntry(name: "dateTimes")
                        })
                    {
                        Context = "/document/finalText/pages/*",
                        Categories = { EntityCategory.Person, EntityCategory.Location, EntityCategory.Datetime, EntityCategory.Organization },
                    },
                    new WebApiSkill(inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: "address")
                            {
                                Source = "/document/finalText/pages/*/locations"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "results"){TargetName ="geolocations"}
                        },
                        uri: string.Format("{0}/api/geo-point-from-name?code={1}", _config.custom_skills_site, _config.custom_skills_key))
                    {
                        Description = "convert location names to gps locations",
                        Context = "/document",
                        BatchSize = 1
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

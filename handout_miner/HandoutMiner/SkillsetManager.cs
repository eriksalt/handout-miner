using Azure.Search.Documents.Indexes.Models;
using handout_miner_shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner
{
    internal class SkillsetManager
    {
        readonly AzureConfig _config;
        public SkillsetManager(AzureConfig config)
        {
            _config = config;
        }

        public SearchIndexerSkillset GetSkillset()
        {
            //2Do: Annotation of images
            //2Do: Facetable?
            //2Do: video processing

            return new SearchIndexerSkillset(
                name: _config.skillset_name,
                skills: new List<SearchIndexerSkill>()
                {
                    //ocr
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
                        ShouldDetectOrientation = true,
                        Description="Perform OCR"
                    },
                    //remove-hyphens from ocr
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
                        Description = "remove hyphens from ocr",
                        Context = "/document/normalized_images/*",
                        BatchSize = 1,
                        Timeout = TimeSpan.FromSeconds(120)

                    },
                    //image tagging
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
                        Details = { ImageDetail.Landmarks },
                        DefaultLanguageCode = ImageAnalysisSkillLanguage.En,
                        Description="Image Reco to generate description/tags"
                    },
                    //merge text and ocr
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
                        Description = "Merge native text content and OCR",
                        Context = "/document"
                    },
                    //merge text and image tags
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
                        Description = "Merge text  with image tags",
                        Context = "/document"
                    },
                    //merge description (caption) into full text
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
                        Description = "Merge img descripton into text",
                        Context = "/document"
                    },
                    //add blob metadata to full text
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
                        Description = "Add blob metadata",
                        Context = "/document",
                        BatchSize = 1,
                        Timeout = TimeSpan.FromSeconds(120)
                    },
                    //split text into pages
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
                        Description = "Split text into pages",
                        Context = "/document/finalText",
                        TextSplitMode = TextSplitMode.Pages,
                        MaximumPageLength = 5000

                    },
                    //lang detection
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
                        })
                    {
                        Description="lang detetion"
                    },
                    //entity recognition
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
                        })
                    {
                        Context = "/document/finalText/pages/*",
                        Categories = { EntityCategory.Person, EntityCategory.Location, EntityCategory.Datetime, EntityCategory.Organization },
                        Description="entity recognition"
                    },
                    //keyphrase extraction
                    new KeyPhraseExtractionSkill(
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
                            new OutputFieldMappingEntry(name: "keyPhrases"),
                        })
                    {
                        Context = "/document/finalText/pages/*",
                        DefaultLanguageCode="en",
                        Description="keyphrase extraction"
                    },
                    //process entities
                    new WebApiSkill(inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.SourceText)
                            {
                                Source = "/document/finalText"
                            },
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.People)
                            {
                                Source = "/document/finalText/pages/*/people"
                            },
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.Phrases)
                            {
                                Source = "/document/finalText/pages/*/keyPhrases"
                            },
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.Locations)
                            {
                                Source = "/document/finalText/pages/*/locations"
                            },
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.Dates)
                            {
                                Source = "/document/finalText/pages/*/dateTimes"
                            }

                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.Locations){TargetName ="normalizedLocations"},
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.Dates){TargetName ="normalizedDates"},
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.People){TargetName ="normalizedPeople"},
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.Phrases){TargetName ="normalizedPhrases"},
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.Geolocations){TargetName ="normalizedGeolocations"}
                        },
                        uri: string.Format("{0}/api/process-entities?code={1}", _config.custom_skills_site, _config.custom_skills_key))
                    {
                        Description = "process entities",
                        Context = "/document",
                        BatchSize = 1,
                        Timeout = TimeSpan.FromSeconds(120)
                    },
                    //hocr data generation
                    new WebApiSkill(inputs: new List<InputFieldMappingEntry>()
                        {
                              new InputFieldMappingEntry(name: "words")
                            {
                                Source = "/document/normalized_images/*/layoutText/words"
                            },
                            new InputFieldMappingEntry(name: "original_height")
                            {
                                Source = "/document/normalized_images/*/originalHeight"
                            },
                            new InputFieldMappingEntry(name: "original_width")
                            {
                                Source = "/document/normalized_images/*/originalWidth"
                            },
                            new InputFieldMappingEntry(name: "normalize_height")
                            {
                                Source = "/document/normalized_images/*/height"
                            },
                            new InputFieldMappingEntry(name: "normalized_width")
                            {
                                Source = "/document/normalized_images/*/width"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: "results"){TargetName ="hocrData"}
                        },
                        uri: string.Format("{0}/api/generate-ocr-data?code={1}", _config.custom_skills_site, _config.custom_skills_key))
                    {
                        Description = "Gen HOCR",
                        Context = "/document",
                        BatchSize = 1,
                        Timeout = TimeSpan.FromSeconds(120)
                    },
                    //merge adventure, session and source into clue source fields
                    new WebApiSkill(inputs: new List<InputFieldMappingEntry>()
                        {
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.AdventureText)
                            {
                                Source = "/document/adventure"
                            },
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.SessionText)
                            {
                                Source = "/document/session"
                            },
                            new InputFieldMappingEntry(name: SkillFieldNames.Inputs.LocationText)
                            {
                                Source = "/document/source"
                            }
                        },
                        outputs: new List<OutputFieldMappingEntry>()
                        {
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.LocationSource){TargetName ="locationSource"},
                            new OutputFieldMappingEntry(name: SkillFieldNames.Outputs.SessionSource){TargetName ="sessionSource"}
                        },
                        uri: string.Format("{0}/api/generate-clue-sources?code={1}", _config.custom_skills_site, _config.custom_skills_key))
                    {
                        Description = "generate clue sources",
                        Context = "/document",
                        BatchSize = 1,
                        Timeout = TimeSpan.FromSeconds(120)
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
